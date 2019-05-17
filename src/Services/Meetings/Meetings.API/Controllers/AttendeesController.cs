using Meetings.API.Helpers;
using Meetings.API.Infrastructure.Core.Repositories;
using Meetings.API.ViewModels;
using Meetings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meetings.API.Controllers
{
    [Route("api/v1/meetings/{meetingId}/attendees")]
    [ApiController]
    public class AttendeesController : ControllerBase
    {
        private readonly IAttendeeRepository _repository;

        public AttendeesController(IAttendeeRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Attendees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendeeViewModel>>> GetAttendees(int meetingId)
        {
            var entites = await _repository.GetAllWithRolesByMeetingAsync(meetingId);

            return Ok(ViewModelHelper.Convert(entites));
        }

        // GET: api/Attendees/5
        [HttpGet("{attendeeId}")]
        public async Task<ActionResult<AttendeeViewModel>> GetAttendee(int meetingId, int attendeeId)
        {
            var entity = await _repository.GetWithRolesAsync(attendeeId);

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
        [HttpPut("{attendeeId}")]
        public async Task<IActionResult> PutAttendee(int meetingId, int attendeeId, AttendeeViewModel model)
        {
            if (attendeeId != model.Id)
            {
                return BadRequest();
            }

            var entity = await _repository.GetAsync(model.Id);

            if (entity == null)
            {
                return NotFound();
            }

            if (meetingId != entity.MeetingId)
            {
                return BadRequest();
            }

            entity.MemberId = model.Member.Id;
            entity.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _repository.CompleteAsync();
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
        public async Task<ActionResult<Attendee>> PostAttendee(int meetingId, AttendeeViewModel model)
        {
            //if (meetingId != model.MeetingId)
            // {
            //    return BadRequest();
            //}

            var entity = new Attendee
            {
                MeetingId = meetingId,
                MemberId = model.Member.Id,
                RoleId = model.Role.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _repository.Add(entity);

            await _repository.CompleteAsync();

            entity = await _repository.GetWithRolesAsync(entity.Id);

            return CreatedAtAction("GetAttendee", new { id = entity.Id }, ViewModelHelper.Convert(entity));
        }

        // DELETE: api/Attendees/5
        [HttpDelete("{attendeeId}")]
        public async Task<ActionResult<AttendeeViewModel>> DeleteAttendee(int meetingId, int attendeeId)
        {
            var entity = await _repository.GetWithRolesAsync(attendeeId);

            if (null == entity)
            {
                return NotFound();
            }

            if (meetingId != entity.MeetingId)
            {
                return BadRequest();
            }

            _repository.Remove(entity);

            await _repository.CompleteAsync();

            return ViewModelHelper.Convert(entity);
        }

        private bool AttendeeExists(int attendeeId)
        {
            return _repository.Exists(attendeeId);
        }
    }
}
