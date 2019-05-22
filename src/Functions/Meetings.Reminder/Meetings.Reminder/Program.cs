using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.IO;

namespace Meetings.Reminder
{
    class Program
    {
        private static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true);

            Configuration = builder.Build();

            Console.WriteLine("Hello World!");

            SendEmail();
        }

        private static void SendEmail()
        {
            var config = Configuration;

            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress(config["SenderName"], config["SenderEmail"]);
            MailboxAddress to = new MailboxAddress("Francis Grignon", "FR@ncis.ca");

            message.From.Add(from);
            message.To.Add(to);
            message.Subject = "This is email subject";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<h1>Hello World!</h1>";
            bodyBuilder.TextBody = "Hello World!";

            message.Body = bodyBuilder.ToMessageBody();
            
            if (false == int.TryParse(config["MailPort"], out int port))
            {
                port = 465;
            }

            using (var client = new SmtpClient())
            {
                client.Connect(config["MailServer"], port, true);
                client.Authenticate(config["SenderEmail"], config["Password"]);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
