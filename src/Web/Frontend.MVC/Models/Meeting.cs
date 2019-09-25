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

        [MaxLength(2048)]
        public string Note { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public ICollection<Attendee> Attendees { get; set; }
    }
}
