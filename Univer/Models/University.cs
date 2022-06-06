using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Univer.Models
{
    public enum ConfirmationStatus
    {
        [Display(Name ="Подтвержден")]
        Confirmed,
        [Display(Name = "Неподтвержден")]
        Unconfirmed
    }
    public class University
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Website { get; set; }
        public string Image { get; set; }
        public virtual User Owner { get; set; }

        public ConfirmationStatus Status { get; set; }
        public virtual ICollection<UniversityIndicator> Indicators { get; set; } 

        
    }
}
