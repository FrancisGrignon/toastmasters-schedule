using Meetings.API.ViewModels;
using Meetings.Models;
using System.Collections.Generic;
using System.Linq;

namespace Meetings.API.Helpers
{
    public static class ViewModelHelper
    {
        public static AttendeeViewModel Convert(Attendee attendee)
        {
            return new AttendeeViewModel
            {
                Id = attendee.Id,
                Role = new RoleViewModel { Id = attendee.RoleId, Name = attendee.Role.Name },
                Member = attendee.MemberId.HasValue ? new MemberViewModel { Id = attendee.MemberId.Value, Name = attendee.Member } : null
            };
        }

        public static IEnumerable<AttendeeViewModel> Convert(IEnumerable<Attendee> attendees)
        {
            return attendees.Select(p => Convert(p)).ToArray();
        }

        public static RoleViewModel Convert(Role role)
        {
            return new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Note = role.Note
            };
        }

        public static IEnumerable<RoleViewModel> Convert(IEnumerable<Role> roles)
        {
            return roles.Select(p => Convert(p)).ToArray();
        }

        public static MeetingViewModel Convert(Meeting meeting, IEnumerable<Attendee> attendees)
        {
            return new MeetingViewModel
            {
                Id = meeting.Id,
                Name = meeting.Name,
                Note = meeting.Note,
                Date = meeting.Date,
                Attendees = Convert(attendees)
            };
        }

        public static MeetingViewModel Convert(Meeting meeting)
        {
            return new MeetingViewModel
            {
                Id = meeting.Id,
                Name = meeting.Name,
                Note = meeting.Note,
                Date = meeting.Date
            };
        }

        public static IEnumerable<MeetingViewModel> Convert(IEnumerable<Meeting> meetings)
        {
            return meetings.Select(p => Convert(p)).ToArray();
        }
    }
}
