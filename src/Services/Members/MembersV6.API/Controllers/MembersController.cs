﻿using Members.DataAccess;
using Members.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MembersV6.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly MemberContext _context;
        private readonly ILogger<MembersController> _logger;

        public MembersController(MemberContext context, ILogger<MembersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers(string? email, int? toastmastersId)
        {
            var query = _context.Members.Where(p => false == p.Deleted);

            if (null != toastmastersId)
            {
                query = query.Where(p => toastmastersId == p.ToastmastersId);
            }

            if (false == string.IsNullOrEmpty(email))
            { 
                query = query.Where(p => email == p.Email);
            }

            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // GET: api/Members/Exists
        [HttpPost("Exists")]
        public async Task<ActionResult<bool>> PostExists(IFormCollection form)
        {
            var email = form["email"];

            var exists = await _context.Members.Where(p => email == p.Email).AnyAsync();

            if (exists)
            {
                return true;
            }

            return false;
        }

        // PUT: api/Members/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            member.Active = model.Active;

            if (string.IsNullOrEmpty(model.Alias))
            {
                if (string.IsNullOrEmpty(member.Alias))
                {
                    member.Alias = model.Name.Substring(0, model.Name.IndexOf(' '));
                }
            }
            else
            {
                member.Alias = model.Alias;
            }

            member.Email = model.Email;
            member.Email2 = model.Email2;
            member.Email3 = model.Email3;
            member.Notify = model.Notify;
            member.Notify2 = model.Notify2;
            member.Notify3 = model.Notify3;
            member.Name = model.Name;
            member.Note = model.Note;
            member.Rank = model.Rank;
            member.ToastmastersId = model.ToastmastersId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            if (string.IsNullOrEmpty(member.Alias))
            {
                member.Alias = member.Name.Substring(0, member.Name.IndexOf(' '));
            }

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Member>> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            member.Deleted = true;

            await _context.SaveChangesAsync();

            return member;
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}
