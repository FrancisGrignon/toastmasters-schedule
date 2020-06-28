using MailKit.Net.Smtp;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Reminders.FunctionApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Reminders.FunctionApp
{
    public class ReminderService : IReminderService
    {
        private IConfiguration Configuration { get; set; }

        private ExecutionContext Context { get; set; }

        private ILogger Log { get; set; }

        public ReminderService(IConfiguration configuration, ILogger log, ExecutionContext context)
        {
            Configuration = configuration;
            Context = context;
            Log = log;
        }

        public async Task<int> Execute()
        {
            Log.LogInformation("Reading meetings");

            var meetings = await new MeetingClient(Configuration).GetPlanning();
            
            var cancelled = meetings[0].Cancelled;

            if (cancelled)
            {
                Log.LogInformation("Meeting cancelled. No need to send emails.");
            }
            else
            {
                string htmlBody = null, textBody;

                var date = meetings[0].Date.ToString("yyyy-MM-dd");
                var theme = meetings[0].Name;
                var subject = $"Les Orateurs - {date} (Thème : {theme})";

                Log.LogInformation("Reading members");

                var memberClient = new MemberClient(Configuration);
                var members = await memberClient.GetAll();

                Log.LogInformation("Preparing calendar");

                var calendar = BuildCalendar(meetings);
                var mailServer = Configuration["MailServer"];

                Log.LogInformation($"Connecting to mail server {mailServer}");

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
                        Log.LogInformation($"Formating email for {member.Name}");

                        // Html function not ready
                        // htmlBody = BuildHtml(calendar, meetings, member);
                        textBody = BuildText(calendar, meetings, member);

                        Log.LogInformation($"Sending email to {member.Name}");

                        SendEmail(client, member, subject, htmlBody, textBody, meetings[0].Id);
                    }

                    client.Disconnect(true);
                }
            }

            return 0;
        }

        private string BuildHtml(Calendar calendar, List<Meeting> meetings, Member member)
        {
            var path = Path.Combine(Context.FunctionAppDirectory, "templates\\email.html");
            var sb = new StringBuilder(File.ReadAllText(path));

            sb.Replace("###name###", member.Alias);
            sb.Replace("###date###", meetings[0].Date.ToString("dddd le dd MMMM yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("fr-CA")));
            sb.Replace("###theme###", meetings[0].Name);
            sb.Replace("###memberid###", member.Id.ToString());

            var buffer = new StringBuilder();
            var myRole = new StringBuilder();

            foreach (var attendee in meetings[0].Attendees)
            {
                if (null == attendee.Member)
                {
                    buffer.AppendLine("<li>");
                    buffer.Append(attendee.Role.Name);
                    buffer.AppendLine("</li>");
                }
                else if (member.Id == attendee.Member.Id)
                {
                    myRole.AppendLine("<li>");
                    myRole.Append(attendee.Role.Name);
                    myRole.AppendLine("</li>");
                }
                else
                {
                    // Ignore
                }
            }

            if (0 == buffer.Length)
            {
                buffer.Append("Tous les rôles ont été comblés.");
            }

            if (0 == myRole.Length)
            {
                buffer.Append("Vous n'avez pas de rôle.");
            }

            sb.Replace("###emptyroles###", buffer.ToString());
            sb.Replace("###myroles###", myRole.ToString());

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

        private string BuildText(Calendar calendar, List<Meeting> meetings, Member member)
        {
            var path = Path.Combine(Context.FunctionAppDirectory, "templates\\email.txt");
            var sb = new StringBuilder(File.ReadAllText(path));

            sb.Replace("###name###", member.Alias);
            sb.Replace("###date###", meetings[0].Date.ToString("dddd le d MMMM yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("fr-CA")));
            sb.Replace("###theme###", meetings[0].Name);
            sb.Replace("###memberid###", member.Id.ToString());

            sb.Replace("###conferenceurl###", Configuration["ConferenceUrl"]);
            sb.Replace("###conferenceid###", Configuration["ConferenceId"]);
            sb.Replace("###conferencepassword###", Configuration["ConferencePassword"]);

            var buffer = new StringBuilder();
            var myRole = new StringBuilder();

            foreach (var attendee in meetings[0].Attendees)
            {
                if (null == attendee.Member)
                {
                    buffer.Append(" - ");
                    buffer.AppendLine(attendee.Role.Name);
                }
                else if (member.Id == attendee.Member.Id)
                {
                    myRole.Append(" - ");
                    myRole.AppendLine(attendee.Role.Name);
                }
                else
                {
                    // Ignore, someone else role
                }
            }

            if (0 == myRole.Length)
            {
                myRole.AppendLine("  Vous n'avez pas de rôles.");
            }

            sb.Replace("###memberroles###", myRole.ToString());

            if (0 == buffer.Length)
            {
                buffer.AppendLine("  Tous les rôles ont été comblés.");
            }

            sb.Replace("###emptyroles###", buffer.ToString());
                
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

        private Calendar BuildCalendar(List<Meeting> meetings)
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

        private void SendEmail(SmtpClient client, Member member, string subject, string htmlBody, string textBody, int meetingId)
        {
            var config = Configuration;

            BodyBuilder bodyBuilder = new BodyBuilder();

            if (null != htmlBody)
            {
                bodyBuilder.HtmlBody = htmlBody;
            }

            if (null != textBody)
            {
                bodyBuilder.TextBody = textBody;
            }

            MimeMessage message = new MimeMessage
            {
                Subject = subject,
                Body = bodyBuilder.ToMessageBody()
            };

            message.Headers.Add(new Header("x-meeting-id", meetingId.ToString()));
            message.Headers.Add(new Header("x-member-id", member.Id.ToString()));
            message.From.Add(new MailboxAddress(config["SenderName"], config["SenderEmail"]));

            bool send = false;

            if (member.Notify && false == string.IsNullOrEmpty(member.Email))
            {
                Log.LogInformation($"Sending email to {member.Email}.");

                var to = new MailboxAddress(member.Name, member.Email);

                message.To.Add(to);

                send = true;
            }
            else
            {
                Log.LogInformation($"{member.Name} don't have a primairy email or don't want to receipt message on that email.");
            }

            if (member.Notify2 && false == string.IsNullOrEmpty(member.Email2))
            {
                Log.LogInformation($"Sending email to {member.Email2}.");

                var to2 = new MailboxAddress(member.Name, member.Email2);

                message.To.Add(to2);

                send = true;
            }
            else
            {
                Log.LogInformation($"{member.Name} don't have a secondary email or don't want to receipt message on that email.");
            }

            if (member.Notify3 && false == string.IsNullOrEmpty(member.Email3))
            {
                Log.LogInformation($"Sending email to {member.Email3}.");

                var to3 = new MailboxAddress(member.Name, member.Email3);

                message.To.Add(to3);

                send = true;
            }
            else
            {
                Log.LogInformation($"{member.Name} don't have a third email or don't want to receipt message on that email.");
            }

            if (send)
            {
                client.Send(message);
            }
        }
    }
}
