using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class Member
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
