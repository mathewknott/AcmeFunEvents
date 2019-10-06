using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AcmeFunEvents.Web.DTO
{
    [Table("AcmeRegistration")]
    public class Registration : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int RegistrationNumber { get; set; }

        public string Comments { get; set; }

        public virtual User User { get; set; }
        
        public virtual Activity Activity { get; set; }

        [JsonIgnore]
        public Guid? UserId { get; set; }

        [JsonIgnore]
        public Guid? ActivityId { get; set; }
    }
}