using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AcmeFunEvents.Web.DTO
{
    [Table("AcmeActivity")]
    public class Activity : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public int Code { get; set; }

        public string Name { get; set; }

        public DateTime? Date { get; set; }
        
    }
}