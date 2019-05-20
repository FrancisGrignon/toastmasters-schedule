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
                new Role() { Id = 1, Active = true, Order = 1000, Name = "Animateur de la rencontre", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 2, Active = true, Order = 1010, Name = "Évaluateur général", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 3, Active = true, Order = 1020, Name = "Mot du jour", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 4, Active = true, Order = 1030, Name = "Toast", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 5, Active = true, Order = 1040, Name = "Farce / Anecdote", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 6, Active = true, Order = 1050, Name = "Meneur de sujets improvisés", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = Role.Improviser, Active = true, Order = 1060, Name = "Improvisateur", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 8, Active = true, Order = 1070, Name = "Discours", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 9, Active = true, Order = 1080, Name = "Évaluateur du discours", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 10, Active = true, Order = 1090, Name = "Grammairien / Compteur d’hésitations", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 11, Active = true, Order = 1110, Name = "Écouteur", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role() { Id = 12, Active = true, Order = 1120, Name = "Chronométreur", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
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
