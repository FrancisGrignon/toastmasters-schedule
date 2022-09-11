using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    public class LogoutController : Controller
    {
        public async Task<ActionResult> Index()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}