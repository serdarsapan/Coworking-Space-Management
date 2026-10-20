using System;
using System.ComponentModel.DataAnnotations;

namespace webProgramming.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Message content is required.")]
        public string Message { get; set; }

        public DateTime DateSent { get; set; }

        // If UserId is null, the notification is meant for ALL users
        public string UserId { get; set; } 
    }
}