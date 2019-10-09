
using Members.DataAccess;
using Members.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Absences.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AbsencesController : ControllerBase
    {
        private readonly MemberContext _context;

        public AbsencesController(MemberContext context)
        {
            _context = context;
        }

        // GET: api/absences
        [HttpGet("members/{memberId:int}/[controller]")]
        public async Task<ActionResult<IEnumerable<Absence>>> GetAbsences(int memberId)
        {
            var query = _context.Absences.Include(p => p.Member).Where(p => false == p.Deleted && p.MemberId == memberId).OrderBy(p => p.StartAt).ThenBy(p => p.EndAt);

            return await query.OrderBy(p => p.StartAt).ToListAsync();
        }

        [HttpGet("[controller]/{date:datetime?}")]
        public async Task<ActionResult<IEnumerable<Absence>>> GetAbsencesAt(DateTime? date)
        {
            var query = _context.Absences.Include(p => p.Member).Where(p => false == p.Deleted);

            if (null == date)
            {
                // Get everything
            }
            else
            {
                query = query.Where(p => p.StartAt <= date && date < p.EndAt.AddDays(1));
            }

            query = query.OrderBy(p => p.StartAt).ThenBy(p => p.EndAt);

            return await query.ToListAsync();
        }


        // GET: api/absences/5
        [HttpGet("members/{memberId:int}/[controller]/{id:int}")]
        public async Task<ActionResult<Absence>> GetAbsence(int memberId, int id)
        {
            var absence = await _context.Absences.Include(p => p.Member).Where(p => p.Id == id).SingleOrDefaultAsync();

            if (absence == null)
            {
                return NotFound();
            }

            if (absence.MemberId != memberId)
            {
                return BadRequest();
            }

            return absence;
        }

        // GET: api/absences/check/2019-01-01
        [HttpGet("members/{memberId:int}/[controller]/check/{date:datetime}")]
        public async Task<ActionResult<bool>> GetAbsenceCheck(int memberId, DateTime date)
        {
            var exists = await _context.Absences.Where(p => false == p.Deleted && memberId == p.MemberId && p.StartAt <= date && date < p.EndAt.AddDays(1)).AnyAsync();

            return exists;
        }

        // PUT: api/absences/5
        [HttpPut("members/{memberId:int}/[controller]/{id:int}")]
        public async Task<IActionResult> PutAbsence(int memberId, int id, Absence model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var absence = await _context.Absences.FindAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            if (model.MemberId != memberId)
            {
                return BadRequest();
            }

            absence.StartAt = model.StartAt;
            absence.EndAt = model.EndAt;
  
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbsenceExists(id))
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

        // POST: api/absences
        [HttpPost("members/{memberId:int}/[controller]")]
        public async Task<ActionResult<Absence>> PostAbsence(int memberId, Absence model)
        {
            var entity = new Absence
            {
                MemberId = memberId,
                StartAt = model.StartAt.Date,
                EndAt = model.EndAt.Date
            };

            _context.Absences.Add(entity);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAbsence", new { memberId, id = model.Id }, model);
        }

        // DELETE: api/absences/5
        [HttpDelete("members/{memberId:int}/[controller]/{id:int}")]
        public async Task<ActionResult<Absence>> DeleteAbsence(int memberId, int id)
        {
            var absence = await _context.Absences.FindAsync(id);

            if (absence == null)
            {
                return NotFound();
            }

            if (absence.MemberId != memberId)
            {
                return BadRequest();
            }

            absence.Deleted = true;

            await _context.SaveChangesAsync();

            return absence;
        }

        private bool AbsenceExists(int id)
        {
            return _context.Absences.Any(e => e.Id == id);
        }
    }
}
