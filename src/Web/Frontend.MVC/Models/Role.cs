using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string Note { get; set; }

        public int Order { get; set; }
    }
}
