using Microsoft.AspNetCore.Mvc;
using ImsMonitoring.Models;
using ImsMonitoring.Data;
using ImsMonitoring.Services;
using Microsoft.EntityFrameworkCore;

namespace ImsMonitoring.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubmissionsController> _logger;

    public SubmissionsController(
        ApplicationDbContext context,
        ILogger<SubmissionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Submission>>> GetSubmissions()
    {
        return await _context.Submissions.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Submission>> GetSubmission(string id)
    {
        var submission = await _context.Submissions.FindAsync(id);

        if (submission == null)
        {
            return NotFound();
        }

        return submission;
    }
}