using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.MVC.Controllers
{
    public class AttendeesController : Controller
    {
        // POST: Attendees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                //return RedirectToAction(nameof(Index));
                return NotFound();
            }
            catch
            {
                return View();
            }
        }

        // GET: Attendees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Attendees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                //return RedirectToAction(nameof(Index));
                return NotFound();
            }
            catch
            {
                return View();
            }
        }        
    }
}