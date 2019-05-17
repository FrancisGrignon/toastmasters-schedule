using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meetings.API.ViewModels
{
    public class MeetingViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Note { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public IEnumerable<AttendeeViewModel> Attendees { get; set; }
    }
}
