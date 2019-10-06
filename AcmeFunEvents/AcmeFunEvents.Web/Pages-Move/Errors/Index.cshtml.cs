using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace AcmeFunEvents.Web.Pages.Errors
{
    [AllowAnonymous]
    public class ErrorModel : PageModel
    {
        private readonly IStringLocalizer<ErrorModel> _localizer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localizer"></param>
        public ErrorModel(IStringLocalizer<ErrorModel> localizer)
        {
            _localizer = localizer;
        }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Title { get; set; }

        public string Description { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Status { get; set; }

        public void OnGet()
        {
            switch (Status) {
                case "400":
                    Title = Status + ": Bad Request";
                    Description = "We are sorry but your request contains bad syntax and cannot be fulfilled..";
                    break;
                case "401":
                    Title = Status + ": Unauthorized";
                    Description = "We are sorry but you are not authorized to access this page..";
                    break;
                case "403":
                    Title = Status + ": Forbidden";
                    Description = "We are sorry but you do not have permission to access this page..";
                    break;
                case "404":
                    Title = Status + ": Page not found";
                    Description = "We are sorry but the page you are looking for was not found..";
                    break;
                case "500":
                    Title = Status + ": Server error";
                    Description = "We are sorry but our server encountered an internal error..";
                    break;
                case "503":
                    Title = Status + ":  Service unavailable";
                    Description = "We are sorry but our service is currently not available..";
                    break;
                default:
                    Title = Status + ": Error";
                    Description = "An Error Occured.";
                    break;
            }

            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}