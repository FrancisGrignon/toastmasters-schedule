using System.ComponentModel.DataAnnotations;

namespace Meetings.API.ViewModels
{
    public class RoleViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
