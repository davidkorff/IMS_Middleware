using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImsMonitoring.Data;
using ImsMonitoring.Models;
using System.Security.Claims;

namespace ImsMonitoring.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/imsinstances")]
    public class ImsInstanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ImsInstanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImsInstance>>> GetInstances()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var instances = await _context.ImsInstances
                .Where(i => i.UserId == userId)
                .ToListAsync();

            return Ok(instances);
        }

        [HttpPost]
        public async Task<ActionResult<ImsInstance>> CreateInstance(ImsInstance instance)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            instance.UserId = userId;
            instance.CreatedAt = DateTime.UtcNow;

            _context.ImsInstances.Add(instance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstances), new { id = instance.Id }, instance);
        }
    }
} 