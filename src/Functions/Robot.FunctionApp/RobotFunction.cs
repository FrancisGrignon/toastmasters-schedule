using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUglify;
using System;

namespace Robot.FunctionApp
{
    public static class RobotFunction
    {
        private static IConfigurationRoot Configuration { get; set; }
        
        private static ILogger Log { get; set; }

        [FunctionName("ReadEmails")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            Log = log;

            var builder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.development.json", optional: true)
                .AddEnvironmentVariables();
               
            Configuration = builder.Build();

            Log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            using (var client = new ImapClient())
            {
                if (false == int.TryParse(Configuration["MailPort"], out int port))
                {
                    port = 993;
                }

                client.Connect(Configuration["MailServer"], port, true);

                client.Authenticate(Configuration["SenderEmail"], Configuration["Password"]);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;

                inbox.Open(FolderAccess.ReadWrite);

                Log.LogInformation("Total messages: {0}", inbox.Count);
                Log.LogInformation("Recent messages: {0}", inbox.Recent);

                var uids = inbox.Search(SearchQuery.NotSeen);

                foreach (var uid in uids)
                {
                    var message = inbox.GetMessage(uid);

                    Log.LogInformation("======================================");

                    foreach (var mailbox in message.From.Mailboxes)
                    {
                        Log.LogInformation(mailbox.Name);
                        Log.LogInformation(mailbox.Address);
                    }

                    Log.LogInformation(message.Subject);
                    Log.LogInformation("--------------------------------------");
                    Log.LogInformation(message.TextBody);
                    Log.LogInformation("--------------------------------------");

                    if (false == string.IsNullOrEmpty(message.HtmlBody))
                    {
                        var result = Uglify.HtmlToText(message.HtmlBody);

                        if (result.HasErrors)
                        {
                            Log.LogWarning("HTML parse errors");

                            foreach (var error in result.Errors)
                            {
                                Log.LogWarning(error.Message);
                            }
                        }

                        Log.LogInformation(result.Code);
                    }

                    inbox.AddFlags(uid, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }
        }       
    }
}
