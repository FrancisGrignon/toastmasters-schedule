using Meetings.Infrastructure;
using Meetings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meetings.API.Controllers
{
    [Route("api/Meetings/{meetingId}/[controller]")]
    [ApiController]
    public class AttendeesController : ControllerBase
    {
        private readonly MeetingContext _context;

        public AttendeesController(MeetingContext context)
        {
            _context = context;
        }

        // GET: api/Attendees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendee>>> GetAttendees(int meetingId)
        {
            return await _context.Attendees.ToListAsync();
        }

        // GET: api/Attendees/5
        [HttpGet("{attendeeId}")]
        public async Task<ActionResult<Attendee>> GetAttendee(int meetingId, int attendeeId)
        {
            var attendee = await _context.Attendees.FindAsync(attendeeId);

            if (attendee == null)
            {
                return NotFound();
            }

            if (meetingId != attendee.MeetingId)
            {
                return BadRequest();
            }

            return attendee;
        }

        // PUT: api/Attendees/5
        [HttpPut("{attendeeId}")]
        public async Task<IActionResult> PutAttendee(int meetingId, int attendeeId, Attendee attendee)
        {
            if (attendeeId != attendee.Id)
            {
                return BadRequest();
            }

            if (meetingId != attendee.MeetingId)
            {
                return BadRequest();
            }

            _context.Entry(attendee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendeeExists(attendeeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Attendees
        [HttpPost]
        public async Task<ActionResult<Attendee>> PostAttendee(int meetingId, Attendee attendee)
        {
            if (meetingId != attendee.MeetingId)
            {
                return BadRequest();
            }

            _context.Attendees.Add(attendee);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttendee", new { id = attendee.Id }, attendee);
        }

        // DELETE: api/Attendees/5
        [HttpDelete("{attendeeId}")]
        public async Task<ActionResult<Attendee>> DeleteAttendee(int meetingId, int attendeeId)
        {
            var attendee = await _context.Attendees.FindAsync(attendeeId);
            if (attendee == null)
            {
                return NotFound();
            }

            if (meetingId != attendee.MeetingId)
            {
                return BadRequest();
            }

            _context.Attendees.Remove(attendee);

            await _context.SaveChangesAsync();

            return attendee;
        }

        private bool AttendeeExists(int attendeeId)
        {
            return _context.Attendees.Any(e => e.Id == attendeeId);
        }
    }
}
