using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class Member
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Notify")]
        public bool Notify { get; set; }

        public string Note { get; set; }

        public bool Active { get; set; }

        public int ToastmastersId { get; set; }

        public string Rank { get; set; }

        [Required]
        public string Alias { get; set; }

        [EmailAddress]
        public string Email2 { get; set; }

        [Display(Name = "Notify")]
        public bool Notify2 { get; set; }

        [EmailAddress]
        public string Email3 { get; set; }

        [Display(Name = "Notify")]
        public bool Notify3 { get; set; }
    }
}
