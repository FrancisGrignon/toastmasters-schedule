using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Members.Scraper
{
    class Program
    {
        private static readonly Uri baseAddress = new Uri("https://www.toastmasters.org");

        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // https://www.toastmasters.org/sitecore/content/Toastmasters/Home/Login?returnUrl=/My-Toastmasters/profile/club-central

            // https://www.toastmasters.org/api/sitecore/ClubRoster/ExportClubRosterToCSVDownload

            int result;

            try
            {
                result = await Login();
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

        public async static Task<int> Login()
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

                       // Console.WriteLine(body.Substring(0, 100));

                        DisplayCookies(cookieContainer);
                    }

                    System.Threading.Thread.Sleep(3000);

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
                        new KeyValuePair<string, string>("Details.UserName", "toastmasters.org@ncis.ca"),
                        new KeyValuePair<string, string>("Details.Password", "tCastle38mc!s"),
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
                        }

                       // Console.WriteLine(body.Substring(0, 100));

                        DisplayCookies(cookieContainer);
                    }                        

                    //make the subsequent web requests using the same HttpClient object
                    

                    Console.WriteLine();
                    Console.WriteLine("Accessing Club Central");

                    using (var result = await client.GetAsync("/My-Toastmasters/profile/club-central"))
                    {                       
                        result.EnsureSuccessStatusCode();

                        //body = await result.Content.ReadAsStringAsync();

                       // Console.WriteLine(body.Substring(0, 100));

                        DisplayCookies(cookieContainer);                        
                    }

                    Console.WriteLine();
                    Console.WriteLine("Downloading members CSV");

                    string csv = null;

                    using (var result = await client.GetAsync("/my-toastmasters/profile/club-central/Club-Roster-CSV"))
                    {
                        result.EnsureSuccessStatusCode();

                        csv = await result.Content.ReadAsStringAsync();

                        //Console.WriteLine(body.Substring(0, 100));

                        DisplayCookies(cookieContainer);
                    }

                    Console.WriteLine();
                    Console.WriteLine("Logout of toastmasters.org");

                    using (var result = await client.GetAsync("/logout"))
                    {
                        result.EnsureSuccessStatusCode();

                       // body = await result.Content.ReadAsStringAsync();

                        //Console.WriteLine(body.Substring(0, 100));

                        DisplayCookies(cookieContainer);
                    }

                    Console.WriteLine();
                    Console.WriteLine("Parsing members CSV");

                    Console.WriteLine();
                    Console.WriteLine("Sharing members");

                }
            }

            Console.ReadKey();

            return 0;
        }
         
        private static void DisplayCookies(CookieContainer cookieContainer)
        {
            Console.WriteLine($"  Cookies");

            foreach (var cookie in cookieContainer.GetCookies(baseAddress).Cast<Cookie>())
            {
                Console.WriteLine($"    {cookie.Name} = {cookie.Value}");
            }
        }

        //public static string ParseVerificationToken(string content)
        //{
        //    // <input name = "__RequestVerificationToken" type = "hidden" value = "pXnJwcEEaq2QfqDjJ2LjdJGpdxhD_CfYaa63_BMIbkcQ9OO-U6ZuTS-cO1GlnXj47MB9WA0DMxqCIDBNpfGFbN_5gyzVL9aKirset_a_vSM1" />
        //    // <input name = "uid" type = "hidden" value = "6376a31d-bf16-4b55-adf7-da7f2a4802f3" />

        //    // Below through the regular expression found hidden domain name = '__ RequestVerificationToken'
        //    string patternRegion = "<\\s*input\\s*.*name\\s*=\\s*\"__RequestVerificationToken\"\\s*.*value\\s*=\\s*\"(?<value>[\\w-]{108,108})\"\\s*/>";
        //    RegexOptions regexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled;
        //    Regex reg = new Regex(patternRegion, regexOptions);
        //    MatchCollection mc = reg.Matches(content);
        //    foreach (Match m in mc)
        //    {
        //        var hidRequestVerificationToken = m.Groups["value"].Value;
        //        //showlables.Content = hidRequestVerificationToken;

        //        return hidRequestVerificationToken;
        //    }

        //    return null
        //}

        //public static string ParseUid(string content)
        //{

        //}
    }
}
