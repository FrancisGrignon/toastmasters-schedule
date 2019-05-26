using Meetings.API.Attributes;
using Meetings.API.Helpers;
using Meetings.API.Infrastructure.Core.Repositories;
using Meetings.API.ViewModels;
using Meetings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meetings.API.Controllers
{
    [Route("api/v1/meetings")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly ILogger<MeetingsController> _logger;
        private readonly IAttendeeRepository _attendeeRepository;
        private readonly IMeetingRepository _meetingRepository;

        public MeetingsController(IMeetingRepository meetingRepository, IAttendeeRepository attendeeRepository, ILogger<MeetingsController> logger)
        {
            _logger = logger;
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

        // GET: api/v1/meetings/planning
        [HttpGet("planning")]
        public async Task<ActionResult<IEnumerable<MeetingViewModel>>> GetMeetingPlanning()
        {
            var entites = await _meetingRepository.GetPlanningWithAttenteesAndRolesAsync(6);

            return Ok(ViewModelHelper.Convert(entites));
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<MeetingViewModel>>> GetMeetingUpcoming()
        {
            var entites = await _meetingRepository.GetPlanningWithAttenteesAndRolesAsync(1);

            if (null == entites || 0 == entites.Length)
            {
                return Ok(null);
            }

            return Ok(ViewModelHelper.Convert(entites[0]));
        }

        // GET: api/Meetings/5
        [HttpGet("{meetingId}")]
        public async Task<ActionResult<MeetingViewModel>> GetMeeting(int meetingId)
        {
            var meeting = await _meetingRepository.GetAsync(meetingId);

            if (meeting == null)
            {
                return NotFound();
            }

            var attendees = await _attendeeRepository.GetAllWithRolesByMeetingAsync(meeting.Id);

            var model = ViewModelHelper.Convert(meeting, attendees);

            return model;
        }

        // PUT: api/Meetings/5
        [ValidateModel]
        [HttpPut("{meetingId}")]
        public async Task<IActionResult> PutMeeting(int meetingId, MeetingRequestViewModel model)
        {
            var entity = await _meetingRepository.GetAsync(meetingId);

            if (null == entity)
            {
                return NotFound();
            }

            entity.Date = model.Date;
            entity.Name = model.Name;
            entity.Note = model.Note;

            _meetingRepository.Update(entity);

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
        [ValidateModel]
        [HttpPost()]
        public async Task<ActionResult<MeetingViewModel>> PostMeeting(MeetingRequestViewModel model)
        {
            var meeting = new Meeting
            {
                Date = model.Date,
                Name = model.Name,
                Note = model.Note,
            };

            _meetingRepository.Add(meeting);

            await _meetingRepository.CompleteAsync();

            var attendees = await _attendeeRepository.GetAllWithRolesByMeetingAsync(meeting.Id);

            var reponse = ViewModelHelper.Convert(meeting, attendees);

            return CreatedAtAction("GetMeeting", new { meetingId = meeting.Id }, reponse);
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
