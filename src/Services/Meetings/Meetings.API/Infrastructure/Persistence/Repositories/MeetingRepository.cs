using Meetings.API.Infrastructure.Core.Repositories;
using Meetings.Infrastructure;
using Meetings.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Meetings.API.Infrastructure.Persistence.Repositories
{
    public class MeetingRepository : Repository<Meeting, MeetingContext>, IMeetingRepository
    {
        public MeetingRepository(MeetingContext context) : base(context)
        {
            // Empty
        }

        public Task<Meeting[]> GetFutureWithAttenteesAndRolesAsync(int numberOfMeetings)
        {
            var startAt = DateTime.UtcNow;

            return Context.Meetings
                .Include(meeting => meeting.Attendees)
                .ThenInclude(attendee => attendee.Role)
                .Where(meeting => startAt <= meeting.Date)
                .OrderBy(meeting => meeting.Date)
                .Take(numberOfMeetings)
                .ToArrayAsync();
        }

        public Task<Meeting> GetWithAttenteesAndRolesAsync(int meetingId)
        {
            return Context.Meetings
                .Include(meeting => meeting.Attendees)
                .ThenInclude(attendee => attendee.Role)
                .Where(meeting => meetingId <= meeting.Id)
                .SingleOrDefaultAsync();
        }

        public override void Add(Meeting entity)
        {
            var roles = Context.Roles.Where(role => role.Active).OrderBy(role => role.Order);

            var attendees = new List<Attendee>();
                       
            foreach (var role in roles)
            {
                // Add four more improviser
                if (Role.Improviser == role.Id)
                {
                    attendees.Add(GenerateAttendee(role));
                    attendees.Add(GenerateAttendee(role));
                    attendees.Add(GenerateAttendee(role));
                    attendees.Add(GenerateAttendee(role));
                }
                else if (Role.Visitor == role.Id)
                {
                    // Ignore
                }
                else if (Role.Member == role.Id)
                {
                    // Ignore
                }
                else
                {
                    attendees.Add(GenerateAttendee(role));
                }
            }

            entity.Attendees = attendees;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            base.Add(entity);
        }

        private Attendee GenerateAttendee(Role role)
        {
            var attendee = new Attendee
            {
                CreatedAt = DateTime.UtcNow,
                Order = role.Order,
                RoleId = role.Id,
                UpdatedAt = DateTime.UtcNow
            };

            return attendee;
        }
    }
}
