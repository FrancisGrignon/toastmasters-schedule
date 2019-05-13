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
                    Name = "Animateur de la rencontre"
                },
                new Role()
                {
                    Name = "Évaluateur général"
                },
                new Role()
                {
                    Name = "Mot du jour"
                },
                new Role()
                {
                    Name = "Toast"
                },
                new Role()
                {
                    Name = "Farce / Anecdote"
                },
                new Role()
                {
                    Name = "Meneur de sujets improvisés"
                },
                new Role()
                {
                    Name = "Improvisateur"
                },
                new Role()
                {
                    Name = "Discours"
                },
                new Role()
                {
                    Name = "Évaluateur du discours"
                },
                new Role()
                {
                    Name = "Grammairien / Compteur d’hésitations"
                },      
                new Role()
                {
                    Name = "Grammarian"
                },
                new Role()
                {
                    Name = "Écouteur"
                },
                new Role()
                {
                    Name = "Chronométreur"
                },
                new Role()
                {
                    Name = "Membre"
                },
                new Role()
                {
                    Name = "Visiteur"
                },
            };
        }
    }
}
