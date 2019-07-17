using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    public class AgendaController : Controller
    {
        private readonly IConfiguration _config;

        public AgendaController(IConfiguration config)
        {
            _config = config;
        }

        // GET: Agenda/{id}
        public async Task<ActionResult> Index(int id)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Get(id);

            if (null == meeting)
            {
                return NotFound();
            }

            return View(meeting);
        }
    }
}