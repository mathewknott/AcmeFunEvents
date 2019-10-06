using System;
using System.Linq;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models.Page;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AcmeFunEvents.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMemoryCache _cache;

        private readonly IActivityService _activityService;

        private readonly IRegistrationService _registrationService;

        private readonly IUserService _userService;

        private readonly ApplicationDbContext _db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="activityService"></param>
        /// <param name="registrationService"></param>
        /// <param name="userService"></param>
        /// <param name="dataContext"></param>
        public AdminController(IMemoryCache memoryCache, IActivityService activityService, IRegistrationService registrationService, IUserService userService, ApplicationDbContext dataContext)
        {
            _cache = memoryCache;
            _activityService = activityService;
            _registrationService = registrationService;
            _userService = userService;
            _db = dataContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new BasePageViewModel();
            return View("Index", model);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Seed()
        {
            if (ModelState.IsValid)
            {
                foreach (var o in _registrationService.GetRegistrationsAsync(out int _).Result)
                {
                    try
                    {
                        _db.Registration.Remove(o);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                foreach (var m in _activityService.GetActivitiesAsync(out int _).Result)
                {
                    if (m != null)
                    {
                        try
                        {
                            _db.Activity.Remove(m);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }

                foreach (var m in _userService.GetUsersAsync(out int _).Result)
                {
                    if (m != null)
                    {
                        try
                        {
                            _db.User.Remove(m);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }

                _db.SaveChanges();

                _cache.Remove(Url.Action("GetActivities", "Activity"));
                _cache.Remove(Url.Action("GetRegistrations", "Activity"));
                _cache.Remove(Url.Action("GetUsers", "Registration"));
                return Content("success");
            }

            return Json(new
            {
                success = "fail",
                errorList = ModelState.Values.SelectMany(v => v.Errors)
            });
        }
    }
}