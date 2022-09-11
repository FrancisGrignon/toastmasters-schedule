using Frontend.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NToastNotify;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    [Authorize]
    public class MembersController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConfiguration _config;
        private readonly IToastNotification _toastNotification;

        public MembersController(IConfiguration config, IToastNotification toastNotification)
        {
            _config = config;
            _toastNotification = toastNotification;
        }

        // GET: Members
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index()
        {
            var client = new MemberClient(_config);

            var Members = await client.GetAll();

            return View(Members);
        }
        
        // GET: Members/Details/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Details(int id)
        {
            var client = new MemberClient(_config);

            var Member = await client.Get(id);

            if (null == Member)
            {
                return NotFound();
            }

            return View(Member);
        }

        public Microsoft.AspNetCore.Mvc.ActionResult Create()
        {
            var Member = new Member();

            return View(Member);
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Create([FromForm]Member model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = new MemberClient(_config);

                    var response = await client.Create(model);

                    if (200 == response.StatusCode)
                    {
                        _toastNotification.AddSuccessToastMessage($"Le member {model.Name} a été ajoutée.");

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

        // GET: Members/Edit/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id)
        {
            var client = new MemberClient(_config);

            var Member = await client.Get(id);

            if (null == Member)
            {
                return NotFound();
            }

            return View(Member);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id, [FromForm]Member model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = new MemberClient(_config);

                    var member = await client.Get(id);

                    if (null == member)
                    {
                        return NotFound();
                    }

                    member.Active = model.Active;
                    member.Alias = model.Alias;
                    member.Email = model.Email;
                    member.Notify = model.Notify;
                    member.Email2 = model.Email2;
                    member.Notify2 = model.Notify2;
                    member.Email3 = model.Email3;
                    member.Notify3 = model.Notify3;
                    member.Name = model.Name;
                    member.Note = model.Note;

                    var response = await client.Update(member);

                    if (200 == response.StatusCode)
                    {
                        _toastNotification.AddSuccessToastMessage($"Le membre {model.Name} a été modifiée.");

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

        // GET: Members/Delete/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(int id)
        {
            var client = new MemberClient(_config);

            var Member = await client.Get(id);

            if (null == Member)
            {
                return NotFound();
            }

            return View(Member);
        }

        // POST: Members/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(int id, [FromForm]Member model)
        {
            try
            {
                var client = new MemberClient(_config);

                var member = await client.Get(id);

                if (null == member)
                {
                    return NotFound();
                }

                var response = await client.Delete(member);

                if (200 == response.StatusCode)
                {
                    _toastNotification.AddSuccessToastMessage($"Le membre {member.Name} a été supprimée.");

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