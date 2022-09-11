using Microsoft.EntityFrameworkCore;
using Polly;

namespace MembersV6.API.Extensions
{
    public static class IWebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                    
                    var retry = Policy.Handle<Exception>()
                            .WaitAndRetry(new TimeSpan[]
                            {
                                TimeSpan.FromSeconds(3),
                                TimeSpan.FromSeconds(5),
                                TimeSpan.FromSeconds(8),
                            });

                    // if the sql server container is not created on run docker compose this
                    // migration can't fail for network related exception. The retry options for DbContext only 
                    // apply to transient exceptions
                    // Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
                    retry.Execute(() => context.Database.Migrate());

                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                }
            }

            return webHost;
        }
    }
}
