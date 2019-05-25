﻿using MailKit.Net.Smtp;
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

            Console.WriteLine("Reading meetings");

            var meetings = await new MeetingClient(Configuration).GetMeetingPlanning();

            Console.WriteLine("Reading members");

            var members = await new MemberClient(Configuration).GetAll();

            string htmlBody, textBody;

            var date = meetings[0].Date.ToString("yyyy-MM-dd");
            var theme = meetings[0].Name;
            var subject = $"Les Orateurs - {date} (Thème : {theme})";

            Console.WriteLine("Preparing calendar");

            var calendar = BuildCalendar(meetings);
            var mailServer = Configuration["MailServer"];

            Console.WriteLine($"Connecting to mail server {mailServer}");

            using (var client = new SmtpClient())
            {
                if (false == int.TryParse(Configuration["MailPort"], out int port))
                {
                    port = 465;
                }

                client.Connect(mailServer, port, true);
                client.Authenticate(Configuration["SenderEmail"], Configuration["Password"]);

                foreach (var member in members)
                {
                    Console.WriteLine($"Formating email for {member.Name}");

                    htmlBody = BuildHtml(calendar, meetings, member);
                    textBody = BuildText(calendar, meetings, member);

                    Console.WriteLine($"Sending email to {member.Name}");

                    SendEmail(client, member, subject, htmlBody, textBody);

                    return 0;
                }

                client.Disconnect(true);
            }

            return 0;
        }

        private static string BuildHtml(Calendar calendar, List<Meeting> meetings, Member member)
        {
            var root = Directory.GetCurrentDirectory();
            var path = Path.Combine(root, "templates\\email.html");
            var sb = new StringBuilder(File.ReadAllText(path));

            sb.Replace("###name###", member.Name);
            sb.Replace("###date###", meetings[0].Date.ToString("yyyy-MM-dd"));
            sb.Replace("###theme###", meetings[0].Name);

            var buffer = new StringBuilder();

            int count = 0;

            foreach (var attendee in meetings[0].Attendees)
            {
                if (null == attendee.Member)
                {
                    buffer.AppendLine("<li>");
                    buffer.Append(attendee.Role.Name);
                    buffer.AppendLine("</li>");

                    count++;
                }
            }

            if (0 == count)
            {
                buffer.Append("Tous les rôles ont été comblés.");
            }

            sb.Replace("###roles###", buffer.ToString());

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

        private static string BuildText(Calendar calendar, List<Meeting> meetings, Member member)
        {
            var root = Directory.GetCurrentDirectory();
            var path = Path.Combine(root, "templates\\email.txt");
            var sb = new StringBuilder(File.ReadAllText(path));

            sb.Replace("###name###", member.Name);
            sb.Replace("###date###", meetings[0].Date.ToString("yyyy-MM-dd"));
            sb.Replace("###theme###", meetings[0].Name);

            var buffer = new StringBuilder();
            var count = 0;

            foreach (var attendee in meetings[0].Attendees)
            {
                if (null == attendee.Member)
                {
                    buffer.Append(" - ");
                    buffer.AppendLine(attendee.Role.Name);

                    count++;
                }
            }

            if (0 == count)
            {
                buffer.Append("Tous les rôles ont été comblés.");
            }

            sb.Replace("###roles###", buffer.ToString());

            buffer.Length = 0;
            
            for (int column = 1; column < calendar.ColumnCount; column++)
            {
                buffer.AppendLine("----------------------------------");

                buffer.Append("Date.: ");
                buffer.AppendLine(calendar[0, column]);
                buffer.Append("Thème: ");
                buffer.AppendLine(calendar[1, column]);
                buffer.AppendLine();

                for (int row = 2; row < calendar.RowCount; row++)
                {
                    buffer.Append("- ");
                    buffer.Append(calendar[row, 0]);
                    buffer.Append(" : ");
                    buffer.AppendLine(calendar[row, column]);
                }

                buffer.AppendLine();
            }
            
            buffer.Append("----------------------------------");

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

        private static void SendEmail(SmtpClient client, Member member, string subject, string htmlBody, string textBody)
        {
            var config = Configuration;

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            };

            MimeMessage message = new MimeMessage
            {
                Subject = subject,
                Body = bodyBuilder.ToMessageBody()
            };

            MailboxAddress from = new MailboxAddress(config["SenderName"], config["SenderEmail"]);
            MailboxAddress to = new MailboxAddress(member.Name, "FR@ncis.ca");

            message.From.Add(from);
            message.To.Add(to);
    
            client.Send(message);
        }
    }
}
