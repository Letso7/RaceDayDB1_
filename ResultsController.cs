using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayDB1.Data;
using RaceDayDB1.Models;

namespace RaceDayDB1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDBContext _context;
        public ResultsController(RaceDayDBContext context) { _context = context; }

        // For Participant My Results page
        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetUserResults(int userId)
        {
            var list = await (from r in _context.Results
                              join e in _context.Events on r.EventId equals e.EventId
                              where r.UserId == userId
                              select new
                              {
                                  EventName = e.EventName,
                                  EventDate = e.EventDate,
                                  FinishTime = r.FinishTime.ToString(),
                                  Position = r.Position,
                                  TotalFinishers = _context.Results.Count(x => x.EventId == r.EventId)
                              }).ToListAsync();
            return Ok(list);
        }

        // Organiser enters results
        [HttpPost]
        public async Task<ActionResult<Result>> PostResult(Result result)
        {
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return Ok(result);
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult> GetEventResults(int eventId)
        {
            return Ok(await _context.Results.Where(r => r.EventId == eventId).ToListAsync());
        }
    }
}