using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class Absence
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Début")]
        public DateTime StartAt { get; set; }

        [Display(Name = "Fin")]
        public DateTime? EndAt { get; set; }

        public Member Member { get; set; }
    }
}
