using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class Facility
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Facility name is required.")]
        [StringLength(100)]
        [Display(Name = "Facility Name")]
        public string Name { get; set; } 

        [Required(ErrorMessage = "Please specify the type (e.g., Office, Meeting-Room, Desk).")]
        public string Type { get; set; }

        [Required]
        [Range(1, 500, ErrorMessage = "Capacity must be a valid number.")]
        public int Capacity { get; set; }

        // Navigation property for bookings linked to this facility
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}