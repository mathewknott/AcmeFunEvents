using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.DTO;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models;
using AcmeFunEvents.Web.Models.Configuration;
using AcmeFunEvents.Web.Models.Page;
using AcmeFunEvents.Web.Models.RegistrationViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AcmeFunEvents.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class RegistrationController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="registrationsContext"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="logger"></param>
        /// <param name="activityService"></param>
        /// <param name="registrationService"></param>
        /// <param name="userService"></param>
        public RegistrationController(
            IMemoryCache cache,
            ApplicationDbContext registrationsContext,
            IOptions<AppOptions> optionsAccessor,
            ILogger<RegistrationController> logger,
            IActivityService activityService,
            IRegistrationService registrationService,
            IUserService userService
            )
        {
            _cache = cache;
            _db = registrationsContext;
            _optionsAccessor = optionsAccessor;
            _logger = logger;
            _activityService = activityService;
            _registrationService = registrationService;
            _userService = userService;
        }

        private readonly IOptions<AppOptions> _optionsAccessor;
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;
        private readonly IActivityService _activityService;
        private readonly IRegistrationService _registrationService;
        private readonly IUserService _userService;

        #region Page Route Methods 
    
        public IActionResult Index()
        {
            return View("Index", new BasePageViewModel());
        }

        public IActionResult Create()
        {
            return View("Create", new RegistrationCreateViewModel
            {
                Activities = _activityService.GetActivitiesAsync(out _).Result.OrderBy(x => x.Name).Select(activity => new SelectListItem { Value = activity.Id.ToString(), Text = activity.Name }).ToList()
            });
        }

        public IActionResult Edit(Guid registrationId)
        {
            var m = _registrationService.GetRegistrationsAsync(out _).Result.SingleOrDefault(x => x.Id.Equals(registrationId));

            if (m != null)
            {
                return View("Update", new RegistrationCreateViewModel
                {
                    Id = registrationId,
                    Comments = m.Comments,
                    ActivityId = m.Activity.Id,
                    FirstName = m.User.FirstName,
                    LastName = m.User.LastName,
                    PhoneNumber = m.User.PhoneNumber,
                    EmailAddress = m.User.EmailAddress,
                    Activities = _activityService.GetActivitiesAsync(out _).Result.OrderBy(x => x.Name).Select(activity => new SelectListItem { Value = activity.Id.ToString(), Text = activity.Name }).ToList()
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

        [HttpPost("/Registration/CreateRegistration")]
        public async Task<IActionResult> Create(RegistrationCreateViewModel form)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _userService.GetUsersAsync(out _).Result.SingleOrDefault(x => x.EmailAddress.Equals(form.EmailAddress, StringComparison.InvariantCultureIgnoreCase));

                var registrations = _registrationService.GetRegistrationsAsync(out _).Result.ToList();

                var registeredUser = registrations.SingleOrDefault(x => x.User.Equals(currentUser) && x.Activity.Id.Equals(form.ActivityId));

                if (registeredUser != null)
                {
                    ModelState.AddModelError("", "User already registered for this activity");
                    return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                }

                var activities = await _activityService.GetActivitiesAsync(out _);

                var findActivity = activities.FirstOrDefault(x => x.Id.Equals(form.ActivityId));

                if (findActivity == null)
                {
                    ModelState.AddModelError("", "Activity Not Found");
                    return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                }

                var m = new Registration
                {
                    Comments = form.Comments,
                    ActivityId = findActivity.Id,
                    //Activity = findActivity
                };

                if (currentUser == null)
                {
                    m.User = new User
                    {
                        FirstName = form.FirstName,
                        LastName = form.LastName,
                        EmailAddress = form.EmailAddress,
                        PhoneNumber = form.PhoneNumber,
                    };

                    _db.User.Add(m.User);
                    _db.SaveChanges();

                    m.UserId = m.User.Id;
                    m.User = m.User;
                }
                else
                {
                    //m.User = currentUser;
                    m.UserId = currentUser.Id;

                    //update properties except for email
                    currentUser.FirstName = form.FirstName;
                    currentUser.LastName = form.LastName;
                    currentUser.PhoneNumber = form.PhoneNumber;
                }

                if (registrations.Any())
                {
                    m.RegistrationNumber = registrations.Last().RegistrationNumber + 1;
                }
                else
                {
                    m.RegistrationNumber = 1;
                }

                TryValidateModel(m);

                if (!ModelState.IsValid)
                {
                    return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                }

                if (_optionsAccessor.Value.EfSettings.DataContext.SaveEntity)
                {
                    try
                    {
                        _db.Registration.Add(m);
                        _db.SaveChanges();

                        _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Registration Created");

                        if (_optionsAccessor.Value.EfSettings.DataContext.SendEmail)
                        {
                            //await SendCreatedConfirmEmail(m);
                        }

                        _cache.Remove(Url.Action("GetRegistrations", "Registration"));
                        return Content("success");
                    }
                    catch (Exception dbEx)
                    {
                        _logger.LogError(new EventId(2), dbEx, "An error occured saving Registration");
                    }
                }
            }
            return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
        }

        [HttpPost("/Registration/UpdateRegistration")]
        public async Task<IActionResult> Update(RegistrationCreateViewModel form)
        {
            if (ModelState.IsValid)
            {
                var m = _registrationService.GetRegistrationsAsync(out int _).Result.SingleOrDefault(x => x.Id.Equals(form.Id));

                if (m != null)
                {
                    var activities = await _activityService.GetActivitiesAsync(out int _);

                    var findActivity = activities.FirstOrDefault(x => x.Id.Equals(form.ActivityId));

                    if (findActivity == null)
                    {
                        ModelState.AddModelError("", "Activity Not Found");
                        return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    }

                    m.ActivityId = findActivity.Id;
                    m.Comments = form.Comments;

                    var currentUser = _userService.GetUsersAsync(out _).Result.SingleOrDefault(x =>x.EmailAddress.Equals(form.EmailAddress, StringComparison.InvariantCultureIgnoreCase));

                    if (currentUser == null)
                    {
                        //create a new user
                        m.User = new User
                        {
                            FirstName = form.FirstName,
                            LastName = form.LastName,
                            EmailAddress = form.EmailAddress,
                            PhoneNumber = form.PhoneNumber
                        };
                    }
                    else
                    {
                        //update properties except for email
                        currentUser.FirstName = form.FirstName;
                        currentUser.LastName = form.LastName;
                        currentUser.PhoneNumber = form.PhoneNumber;
                        m.User = currentUser;
                    }

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
                            _db.Registration.Update(m);
                            _db.SaveChanges();

                            if (_optionsAccessor.Value.EfSettings.DataContext.SendEmail)
                            {
                                //await SendCreatedConfirmEmail(m);
                            }

                            _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Registration Updated");
                            _cache.Remove(Url.Action("GetRegistrations", "Registration"));
                            return Content("success");
                        }
                        catch (Exception dbEx)
                        {
                            _logger.LogError(new EventId(2), dbEx, $"An error occured saving Registration: {form.Id}");
                        }
                    }
                }
            }
            return Json(new { success = "fail", errorList = JsonConvert.SerializeObject(ModelState.Values.Where(x => x.Errors.Count > 0), Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
        }

        [HttpPost("/Registration/DeleteRegistration")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                var registrations = await _registrationService.GetRegistrationsAsync(out _);

                var m = registrations.SingleOrDefault(x => x.Id.Equals(id));

                if (m != null && _optionsAccessor.Value.EfSettings.DataContext.SaveEntity)
                {
                    try
                    {
                        _db.Registration.Remove(m);
                        _db.SaveChanges();
                        _logger.Log(LogLevel.Information, new EventId(2), "", null, (s, exception) => "Registration Deleted");
                        _cache.Remove(Url.Action("GetRegistrations", "Registration"));
                        return Content("success");
                    }
                    catch (Exception dbEx)
                    {
                        _logger.LogError(new EventId(2), dbEx, $"An error occured deleting the Registration: {id}");
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
        [HttpGet("/Registration/GetRegistrations", Name = "Registration_List")]
        public async Task<IActionResult> GetRegistrations(string search, string sort, string order, int limit = 200, int offset = 0)
        {
            var registrations = await _registrationService.GetRegistrationsAsync(sort, out int total, order, limit, offset, search);

            var result = new JsonPagedResult<IEnumerable<Registration>>
            {
                Total = total,
                Rows = registrations
            };

            return Json(result);
        }

        /// <summary>
        /// Returns a list created.
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="search">Text to search</param>
        /// <param name="sort">Parameter to sort </param>
        /// <param name="order">ASC or DESC</param>
        /// <param name="limit">Number of rows to return</param>
        /// <param name="offset">Starting position/offset in table</param>
        /// <returns></returns>
        /// 
        [HttpGet("/Registration/GetRegistrationsByActivityId", Name = "Registration_List_By_ActivityId")]
        public async Task<IActionResult> GetRegistrationsByActivityId(Guid activityId, string search, string sort, string order, int limit = 200, int offset = 0)
        {
            var registrations = await _registrationService.GetRegistrationsByActivityIdAsync(out var total, activityId, limit, offset);

            var result = new JsonPagedResult<IEnumerable<Registration>>
            {
                Total = total,
                Rows = registrations
            };

            return Json(result);
        }

        [HttpGet("/Registration/GetUsers", Name = "Users_List")]
        public async Task<IActionResult> GetUsers(string search, string sort, string order, int limit = 200, int offset = 0)
        {
            var users = await _userService.GetUsersAsync(sort, out int total, order, limit, offset, search);

            var result = new JsonPagedResult<IEnumerable<User>>
            {
                Total = total,
                Rows = users
            };

            return Json(result);
        }
        
        #endregion

    }
}