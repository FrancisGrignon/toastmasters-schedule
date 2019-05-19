using Frontend.MVC.C;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

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

        // GET: Meetings/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Find(id);

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
        public ActionResult Edit(int id)
        {
            return View();
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