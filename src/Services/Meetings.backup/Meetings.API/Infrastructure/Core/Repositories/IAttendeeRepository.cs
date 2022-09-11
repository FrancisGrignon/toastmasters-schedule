using Meetings.Models;
using System.Threading.Tasks;

namespace Meetings.API.Infrastructure.Core.Repositories
{
    public interface IAttendeeRepository : IRepository<Attendee>
    {
        Task<Attendee> GetWithRolesAsync(int id);

        Task<Attendee[]> GetAllWithRolesByMeetingAsync(int meetingId);
    }
}
