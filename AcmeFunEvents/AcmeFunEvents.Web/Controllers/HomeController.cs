using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AcmeFunEvents.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Registration");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            ViewBag.ErrorUrl = feature?.OriginalPath;

            var reExecuteFeature = feature as StatusCodeReExecuteFeature;
            ViewBag.ErrorPathBase = reExecuteFeature?.OriginalPathBase;
            ViewBag.ErrorQuerystring = reExecuteFeature?.OriginalQueryString;

            if (statusCode.HasValue)
            {
                _logger.Log(LogLevel.Error, new EventId(1), "", null, (s, exception) => statusCode + "-" + ViewBag.ErrorUrl);

                if (statusCode == 404 || statusCode == 500)
                {
                    ViewBag.StatusCode = statusCode + " Error";

                    var viewName = statusCode + ".cshtml";
                    return View("~/Views/Shared/ErrorPages/"+ viewName);
                }
            }
            return View("~/Views/Shared/ErrorPages/error.cshtml");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}