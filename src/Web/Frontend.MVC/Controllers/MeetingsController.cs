using Frontend.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NToastNotify;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    [Authorize]
    public class MeetingsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConfiguration _config;
        private readonly IToastNotification _toastNotification;

        public MeetingsController(IConfiguration config, IToastNotification toastNotification)
        {
            _config = config;
            _toastNotification = toastNotification;
        }

        // GET: Meetings
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index()
        {
            var client = new MeetingClient(_config);

            var meetings = await client.GetAll();

            return View(meetings);
        }
        
        // GET: Meetings/Details/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Details(int id)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Get(id);

            if (null == meeting)
            {
                return NotFound();
            }

            return View(meeting);
        }

        public Microsoft.AspNetCore.Mvc.ActionResult Create()
        {
            var meeting = new Meeting();

            return View(meeting);
        }

        // POST: Meetings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Create([FromForm]Meeting model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = new MeetingClient(_config);

                    var response = await client.Create(model);

                    if (200 == response.StatusCode)
                    {
                        _toastNotification.AddSuccessToastMessage($"La rencontre du {model.Date} a été ajoutée.");

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

        // GET: Meetings/Edit/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Get(id);

            if (null == meeting)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // POST: Meetings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id, [FromForm]Meeting model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = new MeetingClient(_config);

                    var meeting = await client.Get(id);

                    if (null == meeting)
                    {
                        return NotFound();
                    }

                    meeting.Name = model.Name;
                    meeting.Note = model.Note;
                    meeting.Date = model.Date;
                    meeting.Cancelled = model.Cancelled;

                    var response = await client.Update(meeting);

                    if (200 == response.StatusCode)
                    {
                        _toastNotification.AddSuccessToastMessage($"La rencontre du {model.Date} a été modifiée.");

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

        // GET: Meetings/Delete/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(int id)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Get(id);

            if (null == meeting)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // POST: Meetings/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(int id, [FromForm]Meeting model)
        {
            try
            {
                var client = new MeetingClient(_config);

                var meeting = await client.Get(id);

                if (null == meeting)
                {
                    return NotFound();
                }

                var response = await client.Delete(meeting);

                if (200 == response.StatusCode)
                {
                    _toastNotification.AddSuccessToastMessage($"La rencontre du {meeting.Date} a été supprimée.");

                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View(model);
            }

            return View(model);
        }
    }
}