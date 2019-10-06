using System.ComponentModel.DataAnnotations.Schema;

namespace AcmeFunEvents.Web.Models.Page
{
    public class BasePageViewModel
    {
        public BasePageViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string Message { get; set; }        
    }
}