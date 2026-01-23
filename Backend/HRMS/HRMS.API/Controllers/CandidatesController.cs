using HRMS.Core.Entities.Recruitment;
using HRMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatesController : ControllerBase
    {
        private readonly HRMSDbContext _context;

        public CandidatesController(HRMSDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var candidates = await _context.Candidates.AsNoTracking().ToListAsync();
            return Ok(candidates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var candidate = await _context.Candidates.AsNoTracking().FirstOrDefaultAsync(c => c.CandidateId == id);
            if (candidate == null) return NotFound();
            return Ok(candidate);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = candidate.CandidateId }, candidate);
        }
    }
}
