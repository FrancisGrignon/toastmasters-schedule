using Meetings.API;
using Meetings.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Meetings.Infrastructure
{
    public class MeetingContextSeed
    {
        public void Seed(MeetingContext context, IHostingEnvironment env, IOptions<MeetingSettings> settings, ILogger<MeetingContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(MeetingContextSeed));

            policy.Execute(() =>
            {
                foreach (var role in GetPreconfiguredRoles())
                {
                    AddOrInsert(context, role);
                }

                context.SaveChanges();
            });
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
                entity.Active = role.Active;
                entity.Name = role.Name;
                entity.Note = role.Note;
                entity.Order = role.Order;

                context.Update(entity);
            }
        }

        private IEnumerable<Role> GetPreconfiguredRoles()
        {
            return new List<Role>()
            {
                new Role() { Id = 1, Order = 1000, Name = "Animateur de la rencontre" },
                new Role() { Id = 2, Order = 1010, Name = "Évaluateur général" },
                new Role() { Id = 3, Order = 1020, Name = "Mot du jour" },
                new Role() { Id = 4, Order = 1030, Name = "Toast" },
                new Role() { Id = 5, Order = 1040, Name = "Farce / Anecdote" },
                new Role() { Id = 6, Order = 1050, Name = "Meneur de sujets improvisés" },
                new Role() { Id = 7, Order = 1060, Name = "Improvisateur" },
                new Role() { Id = 8, Order = 1070, Name = "Discours" },
                new Role() { Id = 9, Order = 1080, Name = "Évaluateur du discours" },
                new Role() { Id = 10, Order = 1090, Name = "Grammairien / Compteur d’hésitations" },      
                new Role() { Id = 11, Order = 1100, Name = "Grammarian" },
                new Role() { Id = 12, Order = 1110, Name = "Écouteur" },
                new Role() { Id = 13, Order = 1120, Name = "Chronométreur" },
                new Role() { Id = 14, Order = 1130, Name = "Membre" },
                new Role() { Id = 15, Order = 1140, Name = "Visiteur" }
            };
        }

        private Policy CreatePolicy(ILogger<MeetingContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetry(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
