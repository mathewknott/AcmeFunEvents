using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.DTO;
using AcmeFunEvents.Web.Extensions;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models;
using AcmeFunEvents.Web.Models.ActivityViewModels;
using AcmeFunEvents.Web.Models.Configuration;
using AcmeFunEvents.Web.Models.Page;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AcmeFunEvents.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="cache"></param>
        /// <param name="emailSender"></param>
        /// <param name="activitiesContext"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="logger"></param>
        /// <param name="activityService"></param>
        /// <param name="registrationService"></param>
        public ActivityController(
            IHostingEnvironment env,
            IMemoryCache cache,
            IEmailSender emailSender,
            ApplicationDbContext activitiesContext,
            IOptions<AppOptions> optionsAccessor,
            ILogger<ActivityController> logger,
            IActivityService activityService,
            IRegistrationService registrationService
            )
        {
            _cache = cache;
            _env = env;
            _emailSender = emailSender;
            _db = activitiesContext;
            _optionsAccessor = optionsAccessor;
            _logger = logger;
            _activityService = activityService;
            _registrationService = registrationService;
        }

        private readonly IOptions<AppOptions> _optionsAccessor;
        private readonly IHostingEnvironment _env;
        private readonly IMemoryCache _cache;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;
        private readonly IActivityService _activityService;
        private readonly IRegistrationService _registrationService;

        #region Page Route Methods 
    
        public IActionResult Index()
        {
            return View("Index", new BasePageViewModel());
        }

        public IActionResult Create()
        {
            return View("Create", new ActivityCreateViewModel());
        }

        public IActionResult Edit(Guid activityId)
        {
            var m = _activityService.GetActivitiesAsync(out int _).Result.SingleOrDefault(x => x.Id.Equals(activityId));

            if (m != null)
            {
                return View("Update", new ActivityCreateViewModel
                {
                    Id = activityId,
                    Code = m.Code,
                    Date = m.Date,
                    Name = m.Name
                });
            }

            return RedirectToAction("Index");
        }
        
        #endregion

        #region POST/PUT Methods 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>

        [HttpPost("/Activity/CreateActivity")]
        public async Task<IActionResult> Create(ActivityCreateViewModel form)
        {
            if (ModelState.IsValid)
            {
                var m = new Activity
                {
                    Name = form.Name,
                    Date = form.Date,
                    Code = form.Code
                };

                var activities = _activityService.GetActivitiesAsync(out int _);

                if (activities.Result.Any())
                {
                    m.Code = activities.Result.Last().Code + 1;
                }
                else
                {
                    m.Code = 1;
                }

                TryValidateModel(m);

                if (!ModelState.IsValid)
                {
                    return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                }

                if (_optionsAccessor.Value.EfSettings.DataContext.SaveEntity)
                {
                    _activityService.AddActivity(m, out var result);

                    if (result > 0)
                    {
                        if (_optionsAccessor.Value.EfSettings.DataContext.SendEmail)
                        {
                            await SendCreatedConfirmEmail(m);
                        }

                        _cache.Remove(Url.Action("GetActivities", "Activity"));

                        return Content("success");
                    }
                }
            }
            return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
        }

        [HttpPost("/Activity/UpdateActivity")]
        public async Task<IActionResult> Update(ActivityCreateViewModel form)
        {
            if (ModelState.IsValid)
            {
                var m = _activityService.GetActivitiesAsync(out int _).Result.SingleOrDefault(x => x.Id.Equals(form.Id));

                if (m != null)
                {
                    m.Name = form.Name;
                    m.Date = form.Date;
                    m.Code = form.Code;

                    TryValidateModel(m);

                    if (!ModelState.IsValid)
                    {
                        return Json(new
                        {
                            success = "fail",
                            errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0),
                                Formatting.None,
                                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
                        });
                    }

                    if (_optionsAccessor.Value.EfSettings.DataContext.SaveEntity)
                    {
                        try
                        {
                            _db.Activity.Update(m);
                            _db.SaveChanges();

                            if (_optionsAccessor.Value.EfSettings.DataContext.SendEmail)
                            {
                                await SendCreatedConfirmEmail(m);
                            }

                            _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Activity Updated");
                            //_cache.Remove(Url.Action("GetActivitys", "Activity"));
                            return Content("success");
                        }
                        catch (Exception dbEx)
                        {
                            _logger.LogError(new EventId(2), dbEx, $"An error occured saving Activity: {form.Id}");
                        }
                    }
                }
            }
            return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
        }

        [HttpPost("/Activity/DeleteActivity")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                var activities = await _activityService.GetActivitiesAsync(out _);

                var m = activities.SingleOrDefault(x => x.Id.Equals(id));

                if (m != null && _optionsAccessor.Value.EfSettings.DataContext.SaveEntity)
                {
                    try
                    {
                        _db.Activity.Remove(m);
                        _db.SaveChanges();
                        _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Activity Deleted");
                        _cache.Remove(Url.Action("GetActivities", "Activity"));
                        return Content("success");
                    }
                    catch (Exception dbEx)
                    {
                        ModelState.AddModelError("","Cannot be deleted. Activity in use.");
                        _logger.LogError(new EventId(2), dbEx, $"An error occured deleting the Activity: {id}");
                    }
                }
            }
            return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
        }

        #endregion

        #region JsonResults 

        /// <summary>
        /// Returns a list created.
        /// </summary>
        /// <param name="search">Text to search</param>
        /// <param name="sort">Parameter to sort </param>
        /// <param name="order">ASC or DESC</param>
        /// <param name="limit">Number of rows to return</param>
        /// <param name="offset">Starting position/offset in table</param>
        /// <returns></returns>
        /// 
        [HttpGet("/Activity/GetActivities", Name = "Activity_List")]
        public async Task<JsonPagedResult<IEnumerable<Activity>>> GetActivities(string search, string sort, string order, int limit = 200, int offset = 0)
        {
            var activities = await _activityService.GetActivitiesAsync(sort, out int total, order, limit, offset, search);

            return new JsonPagedResult<IEnumerable<Activity>>
            {
                Total = total,
                Rows = activities
            };
        }

        /// <summary>
        /// Returns a list created.
        /// </summary>
        /// <param name="search">Text to search</param>
        /// <param name="sort">Parameter to sort </param>
        /// <param name="order">ASC or DESC</param>
        /// <param name="limit">Number of rows to return</param>
        /// <param name="offset">Starting position/offset in table</param>
        /// <returns></returns>
        /// 
        [HttpGet("/Activity/GetRegistrations", Name = "Registrations_List")]
        public async Task<JsonPagedResult<IEnumerable<Registration>>> GetRegistrations(string search, string sort, string order, int limit = 200, int offset = 0)
        {
            var registrations = await _registrationService.GetRegistrationsAsync(sort, out int total, order, limit, offset, search);

            return new JsonPagedResult<IEnumerable<Registration>>
            {
                Total = total,
                Rows = registrations
            };
        }

        #endregion

        #region Utilities 

        /// <summary>
        /// Sends a confimation email to notify the administrator that a Activity has been created
        /// </summary>
        /// <param name="activity">The Activity</param>
        private async Task SendCreatedConfirmEmail(Activity activity)
        {
            return;
            // ignored

            var webRoot = _env.WebRootPath;
            var template = Path.Combine(webRoot, "EmailTemplates/form.htm");
            var emailBody = template.ReadTextFromFile();
            
            emailBody = emailBody.Replace("##Name##", activity.Name);
            await _emailSender.SendEmailAsync(_env.IsProduction() ? _optionsAccessor.Value.DebugEmail : _optionsAccessor.Value.EfSettings.DataContext.NotificationEmail, "Activity Created", emailBody);
        }

        #endregion
    }
}