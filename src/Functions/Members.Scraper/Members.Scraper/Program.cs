using HtmlAgilityPack;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
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

namespace Members.Scraper
{
    class Program
    {
        private static readonly Uri baseAddress = new Uri("https://www.toastmasters.org");
        private static IConfigurationRoot configuration;
        private static IQueueClient queueClient;

        static async Task<int> Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true);

            configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("ServiceBus");
            var queueName = configuration["QueueName"];

            queueClient = new QueueClient(connectionString, queueName);

            int result;

            try
            {
                //var csv = await Login();
                //var members = Parse(csv);

                var generator = new RandomGenerator();

                var members = new List<Member>
                {
                    new Member { Email = "asdf@ncis.ca", Name = generator.RandomString(30, true), ToastmastersId = generator.RandomNumber(0, 9999999) },
                    new Member { Email = "asdf@ncis.ca", Name = generator.RandomString(30, true), ToastmastersId = generator.RandomNumber(0, 9999999) },
                    new Member { Email = "asdf@ncis.ca", Name = generator.RandomString(30, true), ToastmastersId = generator.RandomNumber(0, 9999999) },
                    new Member { Email = "asdf@ncis.ca", Name = generator.RandomString(30, true), ToastmastersId = generator.RandomNumber(0, 9999999) },
                    new Member { Email = "asdf@ncis.ca", Name = generator.RandomString(30, true), ToastmastersId = generator.RandomNumber(0, 9999999) },
                    new Member { Email = "asdf@ncis.ca", Name = generator.RandomString(30, true), ToastmastersId = generator.RandomNumber(0, 9999999) },
                    new Member { Email = "asdf@ncis.ca", Name = generator.RandomString(30, true), ToastmastersId = generator.RandomNumber(0, 9999999) },
                };

                foreach (var member in members)
                {
                    await SendMessagesAsync(member);
                }

                result = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                result = 1;
            }

            return result;
        }

        public static async Task<string> DownloadFile(string url)
        {
            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        return await result.Content.ReadAsStringAsync();
                    }

                }
            }

            return null;
        }

        public static List<Member> Parse(string csv)
        {
            Console.WriteLine();
            Console.WriteLine("Parsing members from CSV");

            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvMemberMapping csvMapper = new CsvMemberMapping();
            CsvParser<Member> csvParser = new CsvParser<Member>(csvParserOptions, csvMapper);

            //var results = csvParser
            //    .ReadFromString(csvReaderOptions, csv)
            //    .ToList();

            var results = csvParser
                 .ReadFromFile(@"C:\Users\livec\Downloads\Club-Roster20190520.csv", Encoding.UTF8)
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

                    Console.WriteLine($"{member.ToastmastersId} | {member.Name} | {member.Rank} | {member.Email}");
                }
                else
                {
                    Console.WriteLine("Invalid");
                }
            }

            return members;
        }

        public async static Task<string> Login()
        {
            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { AllowAutoRedirect = true, UseCookies = true, CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    Console.WriteLine("Accessing toastmasters.org");

                    string body;

                    // usually i make a standard request without authentication, eg: to the home page.
                    // by doing this request you store some initial cookie values, that might be used in the subsequent login request and checked by the server
                    using (var result = await client.GetAsync("/login"))
                    {
                        result.EnsureSuccessStatusCode();

                        body = await result.Content.ReadAsStringAsync();
                    }

                    Console.WriteLine();
                    Console.WriteLine("Logging on toastmasters.org");

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
                            Console.WriteLine("The service is unavailable.");

                            return null;
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("Accessing Club Central");

                    using (var result = await client.GetAsync("/My-Toastmasters/profile/club-central"))
                    {
                        result.EnsureSuccessStatusCode();
                    }

                    Console.WriteLine();
                    Console.WriteLine("Downloading members CSV");

                    string csv = null;

                    using (var result = await client.GetAsync("/my-toastmasters/profile/club-central/Club-Roster-CSV"))
                    {
                        result.EnsureSuccessStatusCode();

                        csv = await result.Content.ReadAsStringAsync();
                    }

                    Console.WriteLine();
                    Console.WriteLine("Logout of toastmasters.org");

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
            Console.WriteLine($"Sending message: {messageBody}");

            // Send the message to the queue
            await queueClient.SendAsync(message);
        }
    }

    public class RandomGenerator
    {
        // Generate a random number between two numbers    
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size    
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random password    
        public string RandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }
    }
}
