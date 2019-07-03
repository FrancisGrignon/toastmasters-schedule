using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Reminders.FunctionApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

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

            Log.LogInformation("Reading members");

            var members = await new MemberClient(Configuration).GetAll();

            string htmlBody = null, textBody;

            var date = meetings[0].Date.ToString("yyyy-MM-dd");
            var theme = meetings[0].Name;
            var subject = $"Les Orateurs - {date} (Thème : {theme})";

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

            return 0;
        }

        private string BuildHtml(Calendar calendar, List<Meeting> meetings, Member member)
        {
            var path = Path.Combine(Context.FunctionAppDirectory, "templates\\email.html");
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

        private string BuildText(Calendar calendar, List<Meeting> meetings, Member member)
        {
            var path = Path.Combine(Context.FunctionAppDirectory, "templates\\email.txt");
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

            MailboxAddress from = new MailboxAddress(config["SenderName"], config["SenderEmail"]);
            MailboxAddress to = new MailboxAddress(member.Name, member.Email);

            var header = new Header("x-meeting-id", meetingId.ToString());

            message.Headers.Add(header);
            message.From.Add(from);
            message.To.Add(to);

            client.Send(message);
        }
    }
}
