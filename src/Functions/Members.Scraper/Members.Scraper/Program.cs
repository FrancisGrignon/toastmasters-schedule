using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TinyCsvParser;

namespace Members.Scraper
{
    class Program
    {
        private static readonly Uri baseAddress = new Uri("https://www.toastmasters.org");

        static async Task<int> Main(string[] args)
        {
            int result;

            try
            {
                var csv = await Login();
                result = Parse(csv);
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

        public static int Parse(string csv)
        {
            Console.WriteLine();
            Console.WriteLine("Parsing members from CSV");

            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvMemberMapping csvMapper = new CsvMemberMapping();
            CsvParser<Member> csvParser = new CsvParser<Member>(csvParserOptions, csvMapper);

            var results = csvParser
                .ReadFromString(csvReaderOptions, csv)
                .ToList();

            //var results = csvParser
           //     .ReadFromFile(@"C:\Users\livec\Downloads\Club-Roster20190520.csv", Encoding.UTF8)
           //     .ToList();

            Member member;

            foreach (var result in results)
            {
                if (result.IsValid)
                {
                    member = result.Result;
                    var tab = member.Name.Split(',');
                    member.Name = tab[0].Trim();

                    if (2 == tab.Length)
                    {
                        member.Title = tab[1].Trim();
                    }

                    Console.WriteLine($"{member.ToastmastersId} | {member.Name} | {member.Title} | {member.Email}");
                }
                else
                {
                    Console.WriteLine("Invalid");
                }
            }

            return 0;
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
                        new KeyValuePair<string, string>("Details.UserName", ""),
                        new KeyValuePair<string, string>("Details.Password", ""),
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
         
        private static void DisplayCookies(CookieContainer cookieContainer)
        {
            Console.WriteLine($"  Cookies");

            foreach (var cookie in cookieContainer.GetCookies(baseAddress).Cast<Cookie>())
            {
                Console.WriteLine($"    {cookie.Name} = {cookie.Value}");
            }
        }
    }
}
