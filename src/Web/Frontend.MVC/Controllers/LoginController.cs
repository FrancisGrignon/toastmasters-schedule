using Frontend.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IToastNotification _toastNotification;

        public LoginController(IConfiguration config, IToastNotification toastNotification)
        {
            _config = config;
            _toastNotification = toastNotification;
        }

        public IActionResult Index(string returnUrl = null)
        {
            var model = new LoginViewModel();

            model.ReturnUrl = returnUrl;

            return View(model);
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([FromForm]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_config["AdminPassword"].Equals(model.Password))
                    {
                        await SignInUser(model.Username, true);

                        if (string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            RedirectToAction("Index", "Calendar");
                        }

                        return Redirect(model.ReturnUrl);
                    }
                }
                catch (Exception)
                {
                    return View(model);
                }
            }

            ModelState.AddModelError(string.Empty, "Courriel ou téléphone ou mot de passe invalide.");

            return View(model);
        }

        /// <summary>  
        /// Sign In User method.  
        /// </summary>  
        /// <param name="username">Username parameter.</param>  
        /// <param name="isPersistent">Is persistent parameter.</param>  
        /// <returns>Returns - await task</returns>  
        private async Task SignInUser(string username, bool isPersistent)
        {
            // Initialization.  
            var claims = new List<Claim>();

            try
            {
                // Setting  
                claims.Add(new Claim(ClaimTypes.Name, username));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //    var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
                //       var authenticationManager = Request.HttpContext;

                var authProperties = new AuthenticationProperties
                {
                    // Empty
                };

                // Sign In.  
                //await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }
            catch (Exception ex)
            {
                // Info  
                throw;
            }
        }
    }
}