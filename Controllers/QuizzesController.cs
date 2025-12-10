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

            if (request.ExistingQuestionsId.Count != 0)
            {
                var existingQuestions = await _db.Questions.Where(a => request.ExistingQuestionsId.Contains(a.Id)).ToListAsync();
                foreach (var eq in existingQuestions)
                {
                    quiz.QuizQuestions.Add(new QuizQuestion
                    {
                        Question = eq,
                        Quiz = quiz
                    });
                }

            }
            foreach (var q in request.Questions)
            {
                var question = new Question { Text = q.Text, Answer = q.Answer };
                quiz.QuizQuestions.Add(new QuizQuestion { Question = question, Quiz = quiz });
            }

            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();


            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz.Id);

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuizRequest request)
        {
            var quiz = await _db.Quizzes.FirstOrDefaultAsync(a => a.Id == id);

            if (quiz == null)
                return NotFound();


            quiz.Title = request.Title ?? quiz.Title;

            _db.QuizQuestions.RemoveRange(quiz.QuizQuestions);
            quiz.QuizQuestions.Clear();
            var allQuestions = await _db.Questions.ToListAsync();

            foreach (var questionId in request.ExistingQuestionsId)
            {
                var existingQuestion = allQuestions.FirstOrDefault(q => q.Id.Equals(questionId));
                if (existingQuestion != null)
                {
                    _db.QuizQuestions.Add(new QuizQuestion { Question = existingQuestion, Quiz = quiz});
                }
            }
            foreach (var question in request.Questions)
            {
                var existingQuestion = allQuestions.FirstOrDefault(q => q.Text == question.Text && q.Answer == question.Answer);
                if (existingQuestion == null)
                {
                    var q = new Question { Text = question.Text, Answer = question.Answer };
                    _db.QuizQuestions.Add(new QuizQuestion { Question = q, Quiz = quiz, QuizId = quiz.Id });
                }
                else
                {
                    _db.QuizQuestions.Add(new QuizQuestion { Question = existingQuestion, Quiz = quiz});

                }
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