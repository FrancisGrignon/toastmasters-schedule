using HtmlAgilityPack;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser;

namespace Members.FunctionApp
{
    public static class WebScaperFunction
    {
        private static readonly Uri baseAddress = new Uri("https://www.toastmasters.org");
        private static IConfigurationRoot configuration;
        private static IQueueClient queueClient;

        private static ILogger Log { get; set; }

        [FunctionName("WebScaper")]
        public static async Task Run([TimerTrigger("0 0 0 * * *")]TimerInfo myTimer, ILogger log)
        {
            Log = log;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.development.json", optional: true)
                .AddEnvironmentVariables();

            configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("ServiceBus");
            var queueName = configuration["QueueName"];

            queueClient = new QueueClient(connectionString, queueName);

            try
            {
                var csv = await Download();
                var members = Parse(csv);

                foreach (var member in members)
                {
                    await SendMessagesAsync(member);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }

            Log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
        
        public static List<Member> Parse(string csv)
        {
            Log.LogInformation("Parsing members from CSV");

            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvMemberMapping csvMapper = new CsvMemberMapping();
            CsvParser<Member> csvParser = new CsvParser<Member>(csvParserOptions, csvMapper);

            var results = csvParser
                .ReadFromString(csvReaderOptions, csv)
                .ToList();

            var members = new List<Member>();
            Member member;
            string[] tab;

            foreach (var result in results)
            {
                if (result.IsValid)
                {
                    member = result.Result;
                    tab = member.Name.Split(',');
                    member.Name = tab[0].Trim();

                    if (2 == tab.Length)
                    {
                        member.Rank = tab[1].Trim();
                    }

                    members.Add(member);

                    Log.LogInformation($"{member.ToastmastersId} | {member.Name} | {member.Rank} | {member.Email}");
                }
                else
                {
                    Log.LogWarning("Invalid line found in CSV.");
                }
            }

            return members;
        }

        public async static Task<string> Download()
        {
            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { AllowAutoRedirect = true, UseCookies = true, CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    Log.LogInformation("Accessing toastmasters.org");

                    string body;

                    // usually i make a standard request without authentication, eg: to the home page.
                    // by doing this request you store some initial cookie values, that might be used in the subsequent login request and checked by the server
                    using (var result = await client.GetAsync("/login"))
                    {
                        result.EnsureSuccessStatusCode();

                        body = await result.Content.ReadAsStringAsync();
                    }

                    Log.LogInformation("Logging on toastmasters.org");

                    var doc = new HtmlDocument();

                    doc.LoadHtml(body);

                    var inputToken = doc.DocumentNode.SelectSingleNode("//*[@name='__RequestVerificationToken']");
                    var token = inputToken.Attributes["value"].Value;

                    var inputUid = doc.DocumentNode.SelectSingleNode("//*[@name='uid']");
                    var uid = inputUid.Attributes["value"].Value;

                    var content = new FormUrlEncodedContent(new[]
                    {
                        //the name of the form values must be the name of <input /> tags of the login form, in this case the tag is <input type="text" name="username">
                        new KeyValuePair<string, string>("__RequestVerificationToken", token),
                        new KeyValuePair<string, string>("uid", uid),
                        new KeyValuePair<string, string>("Details.UserName", configuration["ToastmastersUsername"]),
                        new KeyValuePair<string, string>("Details.Password", configuration["ToastmastersPassword"]),
                        new KeyValuePair<string, string>("Details.MeetingRoomToken", ""),
                    });

                    var secureCookie = new Cookie
                    {
                        Domain = "www.toastmasters.org",
                        Name = "SecureProtocolRequired",
                        Value = "true"
                    };

                    cookieContainer.Add(secureCookie);

                    using (var result = await client.PostAsync("/login", content))
                    {
                        result.EnsureSuccessStatusCode();

                        body = await result.Content.ReadAsStringAsync();

                        if (body.Contains("The service is unavailable."))
                        {
                            Log.LogInformation("The service is unavailable.");

                            return null;
                        }
                    }

                    Log.LogInformation("Accessing Club Central");

                    using (var result = await client.GetAsync("/My-Toastmasters/profile/club-central"))
                    {
                        result.EnsureSuccessStatusCode();
                    }

                    Log.LogInformation("Downloading members CSV");

                    string csv = null;

                    using (var result = await client.GetAsync("/my-toastmasters/profile/club-central/Club-Roster-CSV"))
                    {
                        result.EnsureSuccessStatusCode();

                        csv = await result.Content.ReadAsStringAsync();
                    }

                    Log.LogInformation("Logout of toastmasters.org");

                    using (var result = await client.GetAsync("/logout"))
                    {
                        result.EnsureSuccessStatusCode();
                    }

                    return csv;
                }
            }
        }

        private static async Task SendMessagesAsync(Member member)
        {
            // Create a new message to send to the queue
            string messageBody = JsonConvert.SerializeObject(member, Formatting.Indented);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            // Write the body of the message to the console
            Log.LogInformation($"Sending message: {messageBody}");

            // Send the message to the queue
            await queueClient.SendAsync(message);
        }
    }
}