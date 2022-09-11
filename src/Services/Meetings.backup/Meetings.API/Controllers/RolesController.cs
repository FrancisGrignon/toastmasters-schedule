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
    [Route("api/v1/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly IRoleRepository _repository;

        public RolesController(IRoleRepository repository, ILogger<RolesController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleViewModel>>> GetRoles()
        {
            var entites = await _repository.GetAllAsync();

            return Ok(ViewModelHelper.Convert(entites));
        }

        // GET: api/Roles/5
        [HttpGet("{roleId}")]
        public async Task<ActionResult<RoleViewModel>> GetRole(int roleId)
        {
            var entity = await _repository.GetAsync(roleId);

            if (entity == null)
            {
                return NotFound();
            }

            return ViewModelHelper.Convert(entity);
        }

        // PUT: api/Roles/5
        [ValidateModel]
        [HttpPut("{roleId}")]
        public async Task<IActionResult> PutRole(int roleId, RoleRequestViewModel model)
        {
            if (roleId != model.Id)
            {
                return BadRequest();
            }

            var entity = await _repository.GetAsync(roleId);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;
            entity.Note = model.Note;
            entity.Order = model.Order;

            _repository.Update(entity);

            try
            {
                await _repository.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(roleId))
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

        // POST: api/Roles
        [ValidateModel]
        [HttpPost]
        public async Task<ActionResult<RoleViewModel>> PostRole(RoleRequestViewModel model)
        {
            var entity = new Role
            {
                Name = model.Name,
                Note = model.Note,
                Order = 100
            };

            _repository.Add(entity);

            await _repository.CompleteAsync();

            var response = ViewModelHelper.Convert(entity);

            return CreatedAtAction("GetRole", new { id = model.Id }, response);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{roleId}")]
        public async Task<ActionResult<RoleViewModel>> DeleteRole(int roleId)
        {
            var role = await _repository.GetAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            _repository.Remove(role);

            await _repository.CompleteAsync();

            return ViewModelHelper.Convert(role);
        }

        private bool RoleExists(int roleId)
        {
            return _repository.Exists(roleId);
        }
    }
}
