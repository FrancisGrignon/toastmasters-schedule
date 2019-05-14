using Meetings.Infrastructure;
using Meetings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meetings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly MeetingContext _context;

        public MeetingsController(MeetingContext context)
        {
            _context = context;
        }

        // GET: api/Meetings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetMeeting()
        {
            return await _context.Meetings.ToListAsync();
        }

        // GET: api/Meetings/5
        [HttpGet("{meetingId}")]
        public async Task<ActionResult<Meeting>> GetMeeting(int meetingId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);

            if (meeting == null)
            {
                return NotFound();
            }

            return meeting;
        }

        // PUT: api/Meetings/5
        [HttpPut("{meetingId}")]
        public async Task<IActionResult> PutMeeting(int meetingId, Meeting meeting)
        {
            if (meetingId != meeting.Id)
            {
                return BadRequest();
            }

            _context.Entry(meeting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingExists(meetingId))
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

        // POST: api/Meetings
        [HttpPost]
        public async Task<ActionResult<Meeting>> PostMeeting(Meeting meeting)
        {
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeeting", new { meetingId = meeting.Id }, meeting);
        }

        // DELETE: api/Meetings/5
        [HttpDelete("{meetingId}")]
        public async Task<ActionResult<Meeting>> DeleteMeeting(int meetingId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);
            if (meeting == null)
            {
                return NotFound();
            }

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return meeting;
        }

        private bool MeetingExists(int meetingId)
        {
            return _context.Meetings.Any(e => e.Id == meetingId);
        }
    }
}
