using Meetings.API.Extensions;
using Meetings.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System;

namespace Meetings.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);

                var host = CreateWebHostBuilder(args, configuration);

                host.MigrateDbContext<MeetingContext>((context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();
                    var settings = services.GetService<IOptions<MeetingSettings>>();
                    var logger = services.GetService<ILogger<MeetingContextSeed>>();

                    new MeetingContextSeed().Seed(context, env, settings, logger);
                });

                Log.Information("Starting web host");

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost CreateWebHostBuilder(string[] args, IConfiguration configuration) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
              .Enrich.FromLogContext()
              .WriteTo.Console()
              .WriteTo.Debug()
              .CreateLogger();

            return logger;
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
