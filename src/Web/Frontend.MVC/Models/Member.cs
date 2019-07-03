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
        public string Email { get; set; }

        public string Note { get; set; }

        public bool Active { get; set; }

        public int ToastmastersId { get; set; }

        public string Rank { get; set; }

        public bool Deleted { get; set; }

        [Required]
        public string Alias { get; set; }
    }
}
