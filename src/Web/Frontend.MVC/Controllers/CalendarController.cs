using Frontend.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NToastNotify;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IToastNotification _toastNotification;

        public CalendarController(IConfiguration config, IToastNotification toastNotification)
        {
            _config = config;
            _toastNotification = toastNotification;
        }

        // GET: Calendar
        public async Task<ActionResult> Index()
        {
            var client = new MeetingClient(_config);

            var meetings = await client.GetPlanning();

            var x = meetings.Count + 1;
            var y = meetings[0].Attendees.Count() + 2;
            var calendar = new Calendar(x, y);

            // Add roles
            int m = meetings[0].Attendees.Count();

            calendar[0, 0] = new CalendarCell { AttendeeId = 0, Value = string.Empty };
            calendar[0, 1] = new CalendarCell { AttendeeId = 0, Value = string.Empty };

            int k = 0;

            foreach (var attendee in meetings[0].Attendees)
            {
                calendar[0, k + 2] = new CalendarCell { AttendeeId = 0, Value = attendee.Role.Name };

                k++;
            }

            int i = 0, j = 0;
            string value;

            CalendarCell cell;

            foreach (var meeting in meetings)
            {
                i++;
                j = 0;

                // Add Date
                value = meeting.Date.ToLocalTime().ToString("yyyy-MM-dd");
                calendar[i, 0] = new CalendarCell { MeetingId = meeting.Id, AttendeeId = 0, Value = value };

                // Add subject
                calendar[i, 1] = new CalendarCell { MeetingId = meeting.Id, AttendeeId = 0, Value = meeting.Name };

                j = 2;

                // Add member
                foreach (var attendee in meeting.Attendees)
                {
                    value = attendee.Member?.Name ?? string.Empty;
                    cell = new CalendarCell { MeetingId = meeting.Id, AttendeeId = attendee.Id, Value = value };

                    if (null == attendee.Member)
                    {
                        cell.CanAccept = true;
                    }
                    else
                    {
                        cell.CanRefuse = true;
                    }

                    calendar[i, j] = cell;

                    j++;
                }
            }

            return View(calendar);
        }


        // GET: Calendar
        [Route("[controller]/print")]
        public async Task<ActionResult> Print()
        {
            var client = new MeetingClient(_config);

            var meetings = await client.GetPlanning();

            var x = meetings.Count + 1;
            var y = meetings[0].Attendees.Count() + 2;
            var calendar = new Calendar(x, y);

            // Add roles
            int m = meetings[0].Attendees.Count();

            calendar[0, 0] = new CalendarCell { AttendeeId = 0, Value = string.Empty };
            calendar[0, 1] = new CalendarCell { AttendeeId = 0, Value = string.Empty };

            int k = 0;

            foreach (var attendee in meetings[0].Attendees)
            {
                calendar[0, k + 2] = new CalendarCell { AttendeeId = 0, Value = attendee.Role.Name };

                k++;
            }

            int i = 0, j = 0;
            string value;

            CalendarCell cell;

            foreach (var meeting in meetings)
            {
                i++;
                j = 0;

                // Add Date
                value = meeting.Date.ToLocalTime().ToString("yyyy-MM-dd");
                calendar[i, 0] = new CalendarCell { MeetingId = meeting.Id, AttendeeId = 0, Value = value };

                // Add subject
                calendar[i, 1] = new CalendarCell { MeetingId = meeting.Id, AttendeeId = 0, Value = meeting.Name };

                j = 2;

                // Add member
                foreach (var attendee in meeting.Attendees)
                {
                    value = attendee.Member?.Name ?? string.Empty;
                    cell = new CalendarCell { MeetingId = meeting.Id, AttendeeId = attendee.Id, Value = value };

                    if (null == attendee.Member)
                    {
                        cell.CanAccept = true;
                    }
                    else
                    {
                        cell.CanRefuse = true;
                    }

                    calendar[i, j] = cell;

                    j++;
                }
            }

            return View(calendar);
        }

        public async Task<ActionResult> Accept(int id, int attendeeId)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Get(id);

            if (null == meeting)
            {
                return NotFound();
            }

            var attendee = meeting.Attendees.Where(p => attendeeId == p.Id).SingleOrDefault();

            if (null == attendee)
            {
                return NotFound();
            }

            var model = new AcceptViewModel
            {
                AttendeeId = attendeeId,
                MeetingDate = meeting.Date,
                MeetingId = meeting.Id,
                MeetingName = meeting.Name,
                RoleName = attendee.Role.Name
            };

            var memberClient = new MemberClient(_config);

            var members = await memberClient.GetAll();

            var buffer = Request.Cookies["member"];
            int memberId;

            if (false == int.TryParse(buffer, out memberId))
            {
                memberId = 0;
            }

            model.MemberId = memberId;
            model.Members = members.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Alias }).ToList();

            return View(model);
        }

        // POST: Calendar/Accept/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Accept(int id, [FromForm]AcceptViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var meetingClient = new MeetingClient(_config);
                    var memberClient = new MemberClient(_config);

                    var attendee = await meetingClient.GetAttendee(id, model.AttendeeId);

                    if (null == attendee)
                    {
                        return BadRequest("Attendee not found");
                    }

                    var member = await memberClient.Get(model.MemberId);

                    if (null == member)
                    {
                        return BadRequest("Member not found");
                    }

                    if (null == attendee.Member)
                    {
                        attendee.Member = new Member();
                    }

                    attendee.Member.Id = member.Id;
                    attendee.Member.Name = member.Alias;

                    var response = await meetingClient.UpdateAttendee(id, attendee);

                    if (204 == response.StatusCode)
                    {
                        _toastNotification.AddSuccessToastMessage($"Merci, le role {attendee.Role.Name} a été assigné à {attendee.Member.Name}.");

                        CookieOptions option = new CookieOptions();

                        option.Expires = DateTime.Now.AddYears(1);

                        Response.Cookies.Append("member", member.Id.ToString(), option);

                        return RedirectToAction(nameof(Index));
                    }
                }
                catch
                {
                    return View(model);
                }
            }

            return View(model);
        }

        public async Task<ActionResult> Refuse(int id, int attendeeId)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Get(id);

            if (null == meeting)
            {
                return NotFound();
            }

            var attendee = meeting.Attendees.Where(p => attendeeId == p.Id).SingleOrDefault();

            if (null == attendee)
            {
                return NotFound();
            }

            var model = new RefuseViewModel
            {
                AttendeeId = attendeeId,
                MeetingDate = meeting.Date,
                MeetingId = meeting.Id,
                MeetingName = meeting.Name,
                RoleName = attendee.Role.Name,
                MemberName = attendee.Member.Alias,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Refuse(int id, [FromForm]RefuseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var meetingClient = new MeetingClient(_config);
                    var memberClient = new MemberClient(_config);

                    var attendee = await meetingClient.GetAttendee(id, model.AttendeeId);

                    if (null == attendee)
                    {
                        return BadRequest("Attendee not found");
                    }

                    var member = attendee.Member;

                    attendee.Member = null;

                    var response = await meetingClient.UpdateAttendee(id, attendee);

                    if (204 == response.StatusCode)
                    {
                        _toastNotification.AddSuccessToastMessage($"Merci, le rôle {attendee.Role.Name} a été retiré à {member.Name}.");

                        return RedirectToAction(nameof(Index));
                    }
                }
                catch
                {
                    return View();
                }
            }

            return View(model);
        }        
    }
}