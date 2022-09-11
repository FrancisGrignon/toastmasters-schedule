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

        public Task<Meeting[]> GetPlanningWithAttenteesAndRolesAsync(int numberOfMeetings)
        {
            var startAt = DateTime.UtcNow.AddDays(-1);

            return Context.Meetings
                .Include(meeting => meeting.Attendees)
                .ThenInclude(attendee => attendee.Role)
                .Where(meeting => meeting.Active && startAt <= meeting.Date)
                .OrderBy(meeting => meeting.Date)
                .Take(numberOfMeetings)
                .ToArrayAsync();
        }

        public Task<Meeting> GetWithAttenteesAndRolesAsync(int meetingId)
        {
            return Context.Meetings
                .Include(meeting => meeting.Attendees)
                .ThenInclude(attendee => attendee.Role)
                .Where(meeting => meeting.Active && meetingId == meeting.Id)
                .SingleOrDefaultAsync();
        }

        public override void Add(Meeting entity)
        {
            var roles = Context.Roles.Where(role => role.Active).OrderBy(role => role.Order);

            var attendees = new List<Attendee>();
                       
            foreach (var role in roles)
            {
                if (Role.Improviser == role.Id)
                {
                    // Ignore
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

            base.Add(entity);
        }

        public override void Remove(Meeting meeting)
        {
            if (null == meeting.Attendees)
            {
                // Ignore
            }
            else
            {
                foreach (var attendee in meeting.Attendees)
                {
                    attendee.Active = false;
                    attendee.UpdatedAt = DateTime.UtcNow;
                }
            }

            base.Remove(meeting);
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
