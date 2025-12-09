using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Data;
using QuizAPI.Domain;
using QuizAPI.Models;

namespace QuizAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class QuizzesController(QuizDbContext db) : ControllerBase
    {

        private readonly QuizDbContext _db = db;

        [HttpGet]
        public IActionResult GetAll()
        {
            var quizzes = _db.Quizzes.ToList();
            return Ok(quizzes);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<QuizDetailsResponse>> GetById(Guid id)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            var result = new QuizDetailsResponse
            {
                Title = quiz.Title,
                Questions = quiz.QuizQuestions
                .Select(qq => new QuestionResponse
                {
                    Id = qq.QuestionId,
                    Text = qq.Question.Text
                }).ToList()
            };

            return Ok(result);

        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuizRequest request)
        {
            var quiz = new Quiz
            {
                Title = request.Title,
                QuizQuestions = []
            };

            if (request.ExistingQuestions.Count != 0)
            {
                var existing_questions = await _db.Questions.Where(a => request.ExistingQuestions.Contains(a.Id)).ToListAsync();
                foreach (var eq in existing_questions)
                {
                    //no new one, just join table
                    quiz.QuizQuestions.Add(new QuizQuestion
                    {
                        Question = eq,
                        Quiz = quiz
                    });
                }

            }
            foreach (var q in request.Questions)
            {
                var question = new Question { Text = q.Text, Answer = q.Answer }; //new one then join table
                quiz.QuizQuestions.Add(new QuizQuestion { Question = question, Quiz = quiz });
            }

            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();


            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz.Id);

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuizRequest request)
        {
            var quiz = await _db.Quizzes
                .Include(a => a.QuizQuestions)
                .ThenInclude(aa => aa.Question)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (quiz == null)
                return NotFound();


            quiz.Title = request.Title ?? quiz.Title;

            _db.QuizQuestions.RemoveRange(quiz.QuizQuestions); //remove from join table

            foreach (var question in request.Questions)
            {
                var q = new Question { Text = question.Text, Answer = question.Answer };
                _db.QuizQuestions.Add(new QuizQuestion { Question = q, Quiz = quiz, QuizId = quiz.Id });
            }

            await _db.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var quiz = await _db.Quizzes.FirstOrDefaultAsync(a => a.Id == id);

            if (quiz == null)
                return NotFound();

            await _db.QuizQuestions.Where(a => a.QuizId == id).ExecuteDeleteAsync();
            await _db.Quizzes.Where(a => a.Id == id).ExecuteDeleteAsync();

            return NoContent();
        }
    }
}