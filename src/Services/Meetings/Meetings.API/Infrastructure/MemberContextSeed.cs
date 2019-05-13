using Meetings.Models;
using System.Collections.Generic;

namespace Meetings.Infrastructure
{
    public class MeetingContextSeed
    {
        public void Seed(MeetingContext context)
        {
            foreach (var role in GetPreconfiguredRoles())
            {
                AddOrInsert(context, role);
            }

            context.SaveChanges();
        }

        private void AddOrInsert(MeetingContext context, Role role)
        {
            Role entity = context.Roles.Find(role.Id);

            if (null == entity)
            {
                context.Add(role);
            }
            else
            {
                context.Update(role);
            }
        }

        private IEnumerable<Role> GetPreconfiguredRoles()
        {
            return new List<Role>()
            {
                new Role()
                {
                    Name = "Toastmaster"
                },
                new Role()
                {
                    Name = "Humorist"
                },
                new Role()
                {
                    Name = "Linguist"
                },
                new Role()
                {
                    Name = "Topicsmaster"
                },
                new Role()
                {
                    Name = "Table Topics Speaker"
                },
                new Role()
                {
                    Name = "Meeting Speaker"
                },      
                new Role()
                {
                    Name = "General Evaluator"
                },
                new Role()
                {
                    Name = "Evaluator"
                },
                   new Role()
                {
                    Name = "Grammarian"
                },
                new Role()
                {
                    Name = "Ah - Counter"
                },
                new Role()
                {
                    Name = "Timer"
                },
                new Role()
                {
                    Name = "Visitor"
                },
                new Role()
                {
                    Name = "Member"
                },
            };
        }
    }
}
