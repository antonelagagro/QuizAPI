using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Data;
using QuizAPI.Models;

namespace QuizAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly QuizDbContext _db;
        public QuestionsController(QuizDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public async Task<ActionResult<List<QuestionResponse>>> SearchQuestion([FromQuery] string? searchText)
        {
            var query = _db.Questions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText)) { 
                query = query.Where(q => q.Text.Contains(searchText));
            }

            var results = await query.Select(q => new QuestionResponse
            {
                Text = q.Text,
                Id = q.Id
            }).ToListAsync();

            return Ok(results);
        }
    }
}
