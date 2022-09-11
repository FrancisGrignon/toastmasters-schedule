using System;
using System.ComponentModel.DataAnnotations;

namespace Meetings.API.ViewModels
{
    public class MeetingRequestViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string Note { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool Cancelled { get; set; }
    }
}
