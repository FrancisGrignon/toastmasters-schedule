using Meetings.API.Helpers;
using Meetings.API.Infrastructure.Core.Repositories;
using Meetings.API.ViewModels;
using Meetings.Infrastructure;
using Meetings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meetings.API.Controllers
{
    [Route("api/v1/meetings")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly IAttendeeRepository _attendeeRepository;
        private readonly IMeetingRepository _meetingRepository;

        public MeetingsController(IMeetingRepository meetingRepository, IAttendeeRepository attendeeRepository)
        {
            _meetingRepository = meetingRepository;
            _attendeeRepository = attendeeRepository;
        }

        // GET: api/Meetings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeetingViewModel>>> GetMeeting()
        {
            var entites = await _meetingRepository.GetAllAsync();

            return Ok(ViewModelHelper.Convert(entites));
        }

        // GET: api/Meetings/5
        [HttpGet("{meetingId}")]
        public async Task<ActionResult<MeetingViewModel>> GetMeeting(int meetingId)
        {
            var meeting = await _meetingRepository.GetWithAttenteesAndRolesAsync(meetingId);

            if (meeting == null)
            {
                return NotFound();
            }

            var attendees = await _attendeeRepository.GetAllWithRolesByMeetingAsync(meeting.Id);

            var model = ViewModelHelper.Convert(meeting, attendees);

            return ViewModelHelper.Convert(meeting);
        }

        // PUT: api/Meetings/5
        [HttpPut("{meetingId}")]
        public async Task<IActionResult> PutMeeting(int meetingId, MeetingViewModel model)
        {
            var meeting = await _meetingRepository.GetAsync(meetingId);

            if (null == meeting)
            {
                return NotFound();
            }

            meeting.Name = model.Name;
            meeting.Note = model.Note;
            meeting.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _meetingRepository.CompleteAsync();
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
        [HttpPost()]
        public async Task<ActionResult<MeetingViewModel>> PostMeeting(MeetingViewModel model)
        {
            var meeting = new Meeting
            {
                Date = model.Date,
                Name = model.Name,
                Note = model.Note,
            };

            _meetingRepository.Add(meeting);

            await _meetingRepository.CompleteAsync();

            return CreatedAtAction("GetMeeting", new { meetingId = meeting.Id }, meeting);
        }

        // DELETE: api/Meetings/5
        [HttpDelete("{meetingId}")]
        public async Task<ActionResult<MeetingViewModel>> DeleteMeeting(int meetingId)
        {
            var meeting = await _meetingRepository.GetAsync(meetingId);

            if (meeting == null)
            {
                return NotFound();
            }

            var attendees = await _attendeeRepository.GetAllWithRolesByMeetingAsync(meeting.Id);

            _meetingRepository.Remove(meeting);

            await _meetingRepository.CompleteAsync();
                       
            var model = ViewModelHelper.Convert(meeting, attendees);

            return model;
        }

        private bool MeetingExists(int meetingId)
        {
            return _meetingRepository.Exists(meetingId);
        }
    }
}
