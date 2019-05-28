using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Reminders.FunctionApp
{
    public static class ReminderFunction
    {
        [FunctionName("Reminder")]
        public static async Task Run([TimerTrigger("0 0 10 * * 4,1")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var service = new ReminderService(config, log, context);

            await service.Execute();

            log.LogInformation($"Reminders function executed at: {DateTime.Now}");
        }
    }
}
