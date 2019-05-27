using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class RefuseViewModel
    {
        public string MeetingName { get; set; }

        [Required]
        [HiddenInput]
        public int MeetingId { get; set; }

        public DateTime MeetingDate { get; set; }

        [Required]
        [HiddenInput]
        public int AttendeeId { get; set; }

        public string RoleName { get; set; }

        public string MemberName { get; set; }
    }
}
