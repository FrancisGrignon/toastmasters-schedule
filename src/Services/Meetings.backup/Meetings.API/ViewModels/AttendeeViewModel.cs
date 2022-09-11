using Meetings.Models;

namespace Meetings.API.ViewModels
{
    public class AttendeeViewModel
    {
        public int Id { get; set; }

        public RoleViewModel Role { get; set; }

        public MemberViewModel Member { get; set; }        
    }
}
