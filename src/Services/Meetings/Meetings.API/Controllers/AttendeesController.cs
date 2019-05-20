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
    [Route("api/v1/meetings/{meetingId}/attendees")]
    [ApiController]
    public class AttendeesController : ControllerBase
    {
        private readonly ILogger<AttendeesController> _logger;
        private readonly IAttendeeRepository _attendeeRepository;
        private readonly IMeetingRepository _meetingRepository;

        public AttendeesController(IMeetingRepository meetingRepository, IAttendeeRepository attendeeRepository, ILogger<AttendeesController> logger)
        {
            _logger = logger;
            _meetingRepository = meetingRepository;
            _attendeeRepository = attendeeRepository;
        }

        // GET: api/Attendees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendeeViewModel>>> GetAttendees(int meetingId)
        {
            var entites = await _attendeeRepository.GetAllWithRolesByMeetingAsync(meetingId);

            return Ok(ViewModelHelper.Convert(entites));
        }

        // GET: api/Attendees/5
        [HttpGet("{attendeeId}")]
        public async Task<ActionResult<AttendeeViewModel>> GetAttendee(int meetingId, int attendeeId)
        {
            var exists = _attendeeRepository.Exists(meetingId);

            if (false == exists)
            {
                return BadRequest();
            }

            var entity = await _attendeeRepository.GetWithRolesAsync(attendeeId);

            if (entity == null)
            {
                return NotFound();
            }

            if (meetingId != entity.MeetingId)
            {
                return BadRequest();
            }

            return ViewModelHelper.Convert(entity);
        }

        // PUT: api/Attendees/5
        [ValidateModel]
        [HttpPut("{attendeeId}")]
        public async Task<IActionResult> PutAttendee(int meetingId, int attendeeId, AttendeeRequestViewModel model)
        {
            if (attendeeId != model.Id)
            {
                return BadRequest();
            }

            var exists = _meetingRepository.Exists(meetingId);

            if (false == exists)
            {
                return BadRequest();
            }

            var entity = await _attendeeRepository.GetAsync(model.Id);

            if (entity == null)
            {
                return NotFound();
            }

            if (meetingId != entity.MeetingId)
            {
                return BadRequest();
            }

            entity.MemberId = model.Member.Id;
            entity.Member = model.Member.Name;

            _attendeeRepository.Update(entity);

            try
            {
                await _attendeeRepository.CompleteAsync();
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
        [ValidateModel]
        [HttpPost]
        public async Task<ActionResult<AttendeeViewModel>> PostAttendee(int meetingId, AttendeeRequestViewModel model)
        {
            var exists = _meetingRepository.Exists(meetingId);

            if (false == exists)
            {
                return BadRequest();
            }

            var entity = new Attendee
            {
                MeetingId = meetingId,
                MemberId = model.Member?.Id,
                RoleId = model.RoleId,
            };

            _attendeeRepository.Add(entity);

            await _attendeeRepository.CompleteAsync();

            entity = await _attendeeRepository.GetWithRolesAsync(entity.Id);

            return CreatedAtAction("GetAttendee", new { id = entity.Id }, ViewModelHelper.Convert(entity));
        }

        // DELETE: api/Attendees/5
        [HttpDelete("{attendeeId}")]
        public async Task<ActionResult<AttendeeViewModel>> DeleteAttendee(int meetingId, int attendeeId)
        {
            var entity = await _attendeeRepository.GetWithRolesAsync(attendeeId);

            if (null == entity)
            {
                return NotFound();
            }

            if (meetingId != entity.MeetingId)
            {
                return BadRequest();
            }

            var exists = _meetingRepository.Exists(meetingId);

            if (false == exists)
            {
                return BadRequest();
            }

            _attendeeRepository.Remove(entity);

            await _attendeeRepository.CompleteAsync();

            return ViewModelHelper.Convert(entity);
        }

        private bool AttendeeExists(int attendeeId)
        {
            return _attendeeRepository.Exists(attendeeId);
        }
    }
}
