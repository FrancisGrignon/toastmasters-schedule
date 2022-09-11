using Meetings.API.Infrastructure.Core.Repositories;
using Meetings.Infrastructure;
using Meetings.Models;

namespace Meetings.API.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : Repository<Role, MeetingContext>, IRoleRepository
    {
        public RoleRepository(MeetingContext context) : base(context)
        {
            // Empty
        }
    }
}
