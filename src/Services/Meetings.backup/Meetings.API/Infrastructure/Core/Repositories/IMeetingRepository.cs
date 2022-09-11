using Meetings.Models;
using System.Threading.Tasks;

namespace Meetings.API.Infrastructure.Core.Repositories
{
    public interface IMeetingRepository : IRepository<Meeting>
    {
        Task<Meeting[]> GetPlanningWithAttenteesAndRolesAsync(int numberOfMeetings);

        Task<Meeting> GetWithAttenteesAndRolesAsync(int meetingId);
    }
}
