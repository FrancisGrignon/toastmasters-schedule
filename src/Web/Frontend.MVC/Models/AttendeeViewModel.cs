using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class AttendeeViewModel
    {
        [Required]
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [HiddenInput]
        public int MeetingId { get; set; }

        public int? MemberId { get; set; }

        public List<SelectListItem> Members { get; set; }

        [Required]
        [HiddenInput]
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
