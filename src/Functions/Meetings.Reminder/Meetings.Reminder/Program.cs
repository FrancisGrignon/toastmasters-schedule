using MailKit.Net.Smtp;
using Meetings.Reminder.Models;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Meetings.Reminder
{
    class Program
    {
        private static IConfiguration Configuration { get; set; }

        static async Task<int> Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true);

            Configuration = builder.Build();

            Console.WriteLine("Hello World!");

            var meetings = await new MeetingClient(Configuration).GetMeetingPlanning();
            var members = await new MemberClient(Configuration).GetAll();

            var content = BuildHtml(meetings, members[0]);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "templates/output.html");

            File.WriteAllText(path, content);

            var date = meetings[0].Date.ToString("yyyy-MM-dd");
            var theme = meetings[0].Name;
            var subject = $"Les Orateurs - {date} (Thème : {theme})";

            SendEmail(content, subject, members[0]);

            return 0;
        }

        private static string BuildHtml(List<Meeting> meetings, Member member)
        {
            var root = Directory.GetCurrentDirectory();
            var path = Path.Combine(root, "templates/email.html");
            var sb = new StringBuilder(File.ReadAllText(path));

            sb.Replace("###date###", meetings[0].Date.ToString("yyyy-MM-dd"));
            sb.Replace("###theme###", meetings[0].Name);

            var buffer = new StringBuilder();

            foreach (var attendee in meetings[0].Attendees)
            {
                if (null == attendee.Member)
                {
                    buffer.AppendLine("<li>");
                    buffer.Append(attendee.Role.Name);
                    buffer.AppendLine("</li>");
                }
            }

            sb.Replace("###roles###", buffer.ToString());

            var calendar = BuildCalendar(meetings);

            buffer.Length = 0;

            for (int row = 0; row < calendar.RowCount; row++)
            {
                buffer.AppendLine("<div class=\"row\">");

                for (int column = 0; column < calendar.ColumnCount; column++)
                {
                    if (0 == (row % 2))
                    {
                        buffer.AppendLine("<div class=\"col-sm py-1 px-lg-1 border bg-light\">");
                    }
                    else
                    {
                        buffer.AppendLine("<div class=\"col-sm py-1 px-lg-1 border bg-white\">");
                    }

                    buffer.AppendLine(calendar[row, column]);
                    buffer.AppendLine("</div>");
                }

                buffer.AppendLine("</div>");
            }

            sb.Replace("###calendar###", buffer.ToString());

            return sb.ToString();
        }

        private static Calendar BuildCalendar(List<Meeting> meetings)
        {
            var roles = new List<Role>();

            foreach (var attendee in meetings[0].Attendees)
            {
                roles.Add(attendee.Role);
            }

            var row = roles.Count + 2; 
            var column = meetings.Count + 1;

            var calendar = new Calendar(row, column);

            // Add roles
            int m = roles.Count;

            calendar[0, 0] = string.Empty;
            calendar[1, 0] = string.Empty;

            for (int k = 0; k < m; k++)
            {
                calendar[k + 2, 0] = roles[k].Name;
            }

            column = 0;

            foreach (var meeting in meetings)
            {
                row = 0;
                column++;

                // Add Date
                calendar[0, column] = meeting.Date.ToLocalTime().ToString("yyyy-MM-dd");

                // Add subject
                calendar[1, column] = meeting.Name;

                row = 2;

                foreach (var attendee in meeting.Attendees)
                {
                    calendar[row, column] = attendee.Member?.Name ?? string.Empty;

                    row++;
                }
            }

            return calendar;
        }

        private static void SendEmail(string content, string subject, Member member)
        {
            var config = Configuration;

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = content,
                TextBody = subject
            };

            MimeMessage message = new MimeMessage
            {
                Subject = subject,
                Body = bodyBuilder.ToMessageBody()
            };

            MailboxAddress from = new MailboxAddress(config["SenderName"], config["SenderEmail"]);
            MailboxAddress to = new MailboxAddress("Francis Grignon", "FR@ncis.ca");

            message.From.Add(from);
            message.To.Add(to);
            
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
