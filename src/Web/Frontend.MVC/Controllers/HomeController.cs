using Frontend.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int? m)
        {
            // Save the member id in a cookie
            if (m.HasValue)
            {
                CookieOptions option = new CookieOptions();

                option.Expires = DateTime.Now.AddYears(1);

                Response.Cookies.Append("member", m.ToString(), option);
            }

            return RedirectToAction("Index", "Calendar");
        }

        // GET: M/5
        [HttpGet("/m/{id:int?}")]
        public ActionResult M(int? id)
        {
            // Save the member id in a cookie
            if (id.HasValue)
            {
                CookieOptions option = new CookieOptions();

                option.Expires = DateTime.Now.AddYears(1);

                Response.Cookies.Append("member", id.ToString(), option);
            }

            return RedirectToAction("Index", "Calendar");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
