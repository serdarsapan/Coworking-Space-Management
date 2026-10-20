using System;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int FacilityId { get; set; }
        public virtual Facility Facility { get; set; }

        public string UserId { get; set; } 
        public virtual ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        // Expected values: "Reserved", "Checked-In", "Checked-Out", "Cancelled"
        public string Status { get; set; } 
    }
}