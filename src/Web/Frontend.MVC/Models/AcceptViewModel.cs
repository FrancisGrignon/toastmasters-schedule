using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class AcceptViewModel
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

        [Required(ErrorMessage = "Un membre est requis.")]
        public int MemberId { get; set; }

        public List<SelectListItem> Members { get; set; }
    }
}
