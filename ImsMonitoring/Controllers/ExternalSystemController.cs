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
    [Route("api/[controller]")]
    public class ExternalSystemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExternalSystemController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExternalSystem>>> GetSystems()
        {
            return await _context.ExternalSystems
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        [HttpGet("instance/{imsInstanceId}")]
        public async Task<ActionResult<IEnumerable<ImsInstanceConnection>>> GetInstanceConnections(Guid imsInstanceId)
        {
            return await _context.ImsInstanceConnections
                .Include(c => c.ExternalSystem)
                .Where(c => c.ImsInstanceId == imsInstanceId)
                .ToListAsync();
        }

        [HttpPost("connect")]
        public async Task<ActionResult<ImsInstanceConnection>> CreateConnection(ImsInstanceConnection connection)
        {
            // Verify the IMS instance exists and belongs to the current user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var imsInstance = await _context.ImsInstances
                .FirstOrDefaultAsync(i => i.Id == connection.ImsInstanceId && i.UserId == userId);

            if (imsInstance == null)
                return NotFound("IMS instance not found");

            // Verify the external system exists
            var system = await _context.ExternalSystems
                .FirstOrDefaultAsync(s => s.Id == connection.ExternalSystemId && s.IsActive);

            if (system == null)
                return NotFound("External system not found");

            connection.CreatedAt = DateTime.UtcNow;
            _context.ImsInstanceConnections.Add(connection);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstanceConnections), 
                new { imsInstanceId = connection.ImsInstanceId }, connection);
        }

        [HttpDelete("connect/{id}")]
        public async Task<IActionResult> DeleteConnection(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var connection = await _context.ImsInstanceConnections
                .Include(c => c.ImsInstance)
                .FirstOrDefaultAsync(c => c.Id == id && c.ImsInstance.UserId == userId);

            if (connection == null)
                return NotFound();

            _context.ImsInstanceConnections.Remove(connection);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
} 