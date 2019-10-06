using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AcmeFunEvents.Web.Models.RegistrationViewModels
{
    public class RegistrationCreateViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.Activity)]
        public Guid? ActivityId { get; set; }

        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.FirstName)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.LastName)]
        public string LastName { get; set; }

        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = ResourceKeys.Required)]
        [Display(Name = ResourceKeys.Comments)]
        public string Comments { get; set; }

        public List<SelectListItem> Activities { get; set; }
    }
}