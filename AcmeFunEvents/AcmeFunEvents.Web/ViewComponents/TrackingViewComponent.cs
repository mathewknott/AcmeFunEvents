using System.Threading.Tasks;
using AcmeFunEvents.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AcmeFunEvents.Web.ViewComponents
{
    /// <summary>
    /// 
    /// </summary>
    public class TrackingViewComponent : ViewComponent
    {
        private readonly IHeaderService _headerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerService"></param>
        public TrackingViewComponent(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _headerService.GetGtmAsync());
        }
    }
}