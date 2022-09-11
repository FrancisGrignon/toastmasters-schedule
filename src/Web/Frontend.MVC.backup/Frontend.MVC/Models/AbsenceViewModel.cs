using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class ReportAbsenceViewModel
    {
        [Required]
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }

        public List<SelectListItem> Members { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartAt { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndAt { get; set; }
    }
}
