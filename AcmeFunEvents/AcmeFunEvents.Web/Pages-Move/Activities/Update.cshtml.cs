using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AcmeFunEvents.Web.DTO;
using AcmeFunEvents.Web.Extensions;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AcmeFunEvents.Web.Pages.Activities
{
    public class UpdateModel : PageModel
    {
        [BindProperty]
        public Activity Activity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="cache"></param>
        /// <param name="emailSender"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="activityService"></param>
        public UpdateModel(
            IHostingEnvironment env,
            IMemoryCache cache,
            IEmailSender emailSender,
            IOptions<AppOptions> optionsAccessor,
            IActivityService activityService
        )
        {
            _cache = cache;
            _env = env;
            _emailSender = emailSender;
            _optionsAccessor = optionsAccessor;
            _activityService = activityService;
        }

        private readonly IActivityService _activityService;
        
        private readonly IMemoryCache _cache;
        private readonly IOptions<AppOptions> _optionsAccessor;
        private readonly IHostingEnvironment _env;
        private readonly IEmailSender _emailSender;

        public void OnGet(Guid activityId, string returnUrl = null)
        {
            var m = _activityService.GetActivitiesAsync(out int _).Result.SingleOrDefault(x => x.Id.Equals(activityId));

            if (m != null)
            {
                Activity.Id = activityId;
                Activity.Code = m.Code;
                Activity.Date = m.Date;
                Activity.Name = m.Name;
            }

            ViewData["ReturnUrl"] = returnUrl ?? Url.Page("/Activities/Index");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            TryValidateModel(Activity);

            if (ModelState.IsValid)
            {
                var m = new Activity
                {
                    Name = Activity.Name,
                    Date = Activity.Date,
                    Code = Activity.Code
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
                    return Page();
                }

                if (_optionsAccessor.Value.EfSettings.DataContext.SaveEntity)
                {
                    _activityService.AddActivity(Activity, out var result);

                    if (result > 0)
                    {
                        if (_optionsAccessor.Value.EfSettings.DataContext.SendEmail)
                        {
                            await SendCreatedConfirmEmail(Activity);
                        }

                        _cache.Remove(Url.Action("GetActivities", "Activity"));

                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            
            return Page();
        }

        #region Utilities 

        /// <summary>
        /// Sends a confimation email to notify the administrator that a Movie has been created
        /// </summary>
        /// <param name="activity">The Activity</param>
        private async Task SendCreatedConfirmEmail(Activity activity)
        {
            var webRoot = _env.WebRootPath;
            var template = Path.Combine(webRoot, "EmailTemplates/form.htm");
            var emailBody = template.ReadTextFromFile();
           
            emailBody = emailBody.Replace("##Name##", activity.Name);
            await _emailSender.SendEmailAsync(_env.IsProduction() ? _optionsAccessor.Value.DebugEmail : _optionsAccessor.Value.EfSettings.DataContext.NotificationEmail, "Movie Created", emailBody);
        }

        #endregion
    }
}