using Meetings.API.Infrastructure.Core.Repositories;
using Meetings.Infrastructure;
using Meetings.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Meetings.API.Infrastructure.Persistence.Repositories
{
    public class AttendeeRepository : Repository<Attendee, MeetingContext>, IAttendeeRepository
    {
        public AttendeeRepository(MeetingContext context) : base(context)
        {
            // Empty
        }

        public Task<Attendee> GetWithRolesAsync(int id)
        {
            return Context.Attendees.Include(p => p.Role).Where(p => id == p.Id).SingleOrDefaultAsync();
        }

        public Task<Attendee[]> GetAllWithRolesByMeetingAsync(int meetingId)
        {
            return Context
                .Attendees
                .Include(attendee => attendee.Role)
                .Where(attendee => meetingId == attendee.MeetingId)
                .OrderBy(p => p.Order)
                .ToArrayAsync();
        }
    }
}
