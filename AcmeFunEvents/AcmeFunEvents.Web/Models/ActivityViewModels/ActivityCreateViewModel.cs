using System;
using System.ComponentModel.DataAnnotations;

namespace AcmeFunEvents.Web.Models.ActivityViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityCreateViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.Name)]
        public string Name { get; set; }

        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.Date)]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.Code)]
        public int Code { get; set; }
    }
}