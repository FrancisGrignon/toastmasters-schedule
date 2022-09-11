using Frontend.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Frontend.MVC.Controllers
{
    public class AgendaController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConfiguration _config;

        public AgendaController(IConfiguration config)
        {
            _config = config;
        }

        // GET: Agenda/{id}
        [HttpGet("Agenda/{id:int}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(int id)
        {
            var client = new MeetingClient(_config);

            var meeting = await client.Get(id);

            if (null == meeting)
            {
                return NotFound();
            }

            var model = new AgendaViewModel
            {
                Date = meeting.Date,
                Name = meeting.Name,
                Note = meeting.Note
            };

            foreach (var attendee in meeting.Attendees)
            {
                switch (attendee.Role.Id)
                {
                    case Role.Toastmaster:
                        model.Toastmaster = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.GeneralEvaluator:
                        model.GeneralEvaluator = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.WordOfTheDay:
                        model.WordOfTheDay = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.Toast:
                        model.Toast = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.Humour:
                        model.Humour = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.TopicsMaster:
                        model.TopicsMaster = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.Speaker:
                        model.Speaker = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.Evaluator:
                        model.Evaluator = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.Grammarian:
                        model.Grammarian = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.Listener:
                        model.Listener = attendee.Member?.Name ?? string.Empty;
                        break;
                    case Role.Timer:
                        model.Timer = attendee.Member?.Name ?? string.Empty;
                        break;
                }
            }

            return View(model);
        }
    }
}