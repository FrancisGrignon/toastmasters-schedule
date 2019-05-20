using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using Frontend.MVC.Models;

namespace Frontend.MVC.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly IConfiguration _config;

        public MeetingsController(IConfiguration config)
        {
            _config = config;
        }

        // GET: Meetings
        public async Task<ActionResult> Index()
        {
            var client = new MeetingClient(_config);

            var meetings = await client.GetAll();

            return View(meetings);
        }

        // GET: Meetings
        public async Task<ActionResult> Planning()
        {
            var client = new MeetingClient(_config);

            var roles = await client.GetRoles();
            var meetings = await client.GetPlanning();
            

            var x = meetings.Count + 1;
            var y = roles.Count + 2;
            var cell = new Planning(x, y);

            // Add roles
            int m = roles.Count;

            cell[0, 0] = string.Empty;
            cell[0, 1] = string.Empty;

            for (int k = 0; k < m; k++)
            {
                cell[0, k + 2] = roles[k].Name;
            }

            int i = 0, j = 0;

            foreach (var meeting in meetings)
            {
                i++;
                j = 0;

                // Add Date
                cell[i, 0] = meeting.Date.ToLocalTime().ToString("yyyy-MM-dd");

                // Add subject
                cell[i, 1] = meeting.Name;

                j = 2;

                foreach (var attendee in meeting.Attendees)
                {
                    cell[i, j] = attendee.Member?.Name ?? string.Empty;

                    j++;
                }
            }

            return View(cell);
        }

        // GET: Meetings
        public async Task<ActionResult> Planning2()
        {
            var client = new MeetingClient(_config);

            var roles = await client.GetRoles();
            var meetings = await client.GetPlanning();
            var x = meetings.Count + 1; 
            var y = roles.Count + 2;

            var cell = new string[x, y];

            // Add roles
            int m = roles.Count;

            cell[0, 0] = string.Empty;
            cell[0, 1] = string.Empty;

            for (int k = 0; k < m; k++)
            {
                cell[0, k + 2] = roles[k].Name;
            }

            int i = 0, j = 0;

            foreach (var meeting in meetings)
            {
                i++;
                j = 0;

                // Add Date
                cell[i, 0] = meeting.Date.ToLocalTime().ToString("yyyy-MM-dd HH:mm");

                // Add subject
                cell[i, 1] = meeting.Name;

                j = 2;

                foreach (var attendee in meeting.Attendees)
                {
                    cell[i, j] = attendee.Member?.Name ?? string.Empty;

                    j++;
                }
            }

            return View(cell);
        }

        // GET: Meetings/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Get(id);

            if (null == meeting)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // POST: Meetings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Meetings/Edit/5
        public async Task<ActionResult> Edit(int id)
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
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Meetings/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Meetings/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}