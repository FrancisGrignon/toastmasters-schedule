using Frontend.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NToastNotify;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    [Route("meetings/{meetingId:int}/attendees")]
    [ApiController]
    public class AttendeesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IToastNotification _toastNotification;

        public AttendeesController(IConfiguration config, IToastNotification toastNotification)
        {
            _config = config;
            _toastNotification = toastNotification;
        }

        // GET: Attendees/Edit/5
        [HttpGet("{attendeeId}")]
        public async Task<ActionResult> Edit(int meetingId, int attendeeId)
        {
            var client = new MeetingClient(_config);
            var attendee = await client.GetAttendee(meetingId, attendeeId);

            if (null == attendee)
            {
                return NotFound();
            }

            var model = new AttendeeViewModel
            {
                Id = attendeeId,
                MeetingId = meetingId,
                RoleId = attendee.Role.Id,
                RoleName = attendee.Role.Name
            };

            if (null == attendee.Member)
            {
                model.MemberId = 0;
            }
            else
            {
                model.MemberId = attendee.Member.Id;
            }

            var memberClient = new MemberClient(_config);
            var members = await memberClient.GetAll();

            model.Members = members.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Alias }).ToList();

            return View(model);
        }

        // POST: Attendees/Edit/5
        [HttpPost("{attendeeId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int meetingId, int attendeeId, [FromForm]AttendeeViewModel model)
        {
            var memberClient = new MemberClient(_config);
            var members = await memberClient.GetAll();

            if (ModelState.IsValid)
            {
                try
                {
                    var client = new MeetingClient(_config);

                    var attendee = await client.GetAttendee(meetingId, attendeeId);

                    if (null == attendee)
                    {
                        return NotFound();
                    }

                    if (null == model.MemberId)
                    {
                        attendee.Member = null;
                    }
                    else
                    {
                        attendee.Member = await memberClient.Get(model.MemberId.Value);
                    }

                    var response = await client.UpdateAttendee(meetingId, attendee);

                    if (response.IsSuccessStatusCode)
                    {
                        _toastNotification.AddSuccessToastMessage($"Le participant a été modifiée.");

                        return RedirectToAction("Details", "Meetings", new { Id = meetingId });
                    }
                }
                catch
                {
                    model.Members = members.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Alias }).ToList();

                    return View(model);
                }
            }

            model.Members = members.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Alias }).ToList();

            return View(model);
        }        
    }
}