using System.ComponentModel.DataAnnotations;

namespace Meetings.API.ViewModels
{
    public class AttendeeRequestViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int RoleId { get; set; }

        public MemberViewModel Member { get; set; }        
    }
}
