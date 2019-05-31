using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class Meeting
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Thème")]
        public string Name { get; set; }

        [Required]
        [MaxLength(2048)]
        public string Note { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public ICollection<Attendee> Attendees { get; set; }
    }
}
