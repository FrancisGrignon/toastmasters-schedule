using Members.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Members.DataAccess
{
    public class MemberContextSeed
    {
        public void Seed(MemberContext context)
        {
            foreach (var role in GetPreconfiguredRoles())
            {
                AddOrInsert(context, role);
            }

            context.SaveChanges();
        }

        private void AddOrInsert(MemberContext context, Role role)
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
                }
            };
        }
    }
}
