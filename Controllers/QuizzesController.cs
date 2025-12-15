using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Data;
using QuizAPI.Domain;
using QuizAPI.Models;
using QuizAPI.Export;

namespace QuizAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class QuizzesController(QuizDbContext db, QuizExportService exportService) : ControllerBase
    {

        private readonly QuizDbContext _db = db;
        private readonly QuizExportService _exportService = exportService;
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 200);


            var quizzes = await _db.Quizzes
             .OrderBy(q => q.Title)
             .Skip((page - 1) * pageSize)
             .Take(pageSize)
             .Select(q => new QuizListItemResponse
             {
                 Id = q.Id,
                 Title = q.Title
             })
             .ToListAsync();

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

            if (request.ExistingQuestionsId != null && request.ExistingQuestionsId.Count > 0)
            {
                var existingQuestions = await _db.Questions.Where(a => request.ExistingQuestionsId.Contains(a.Id)).ToListAsync();
                foreach (var eq in existingQuestions)
                {
                    quiz.QuizQuestions.Add(new QuizQuestion
                    {
                        QuestionId = eq.Id
                    });
                }

            }
            if (request.Questions != null && request.Questions.Count > 0)
            {
                foreach (var q in request.Questions)
                {
                    var question = new Question { Text = q.Text, Answer = q.Answer };
                    quiz.QuizQuestions.Add(new QuizQuestion { Question = question });
                }
            }


            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();


            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz.Id);

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuizRequest request)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.QuizQuestions)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (quiz == null)
                return NotFound();


            quiz.Title = request.Title ?? quiz.Title;

            await _db.QuizQuestions
                .Where(q => q.QuizId == id)
                .ExecuteDeleteAsync();
            quiz.QuizQuestions.Clear();

            if (request.ExistingQuestionsId != null && request.ExistingQuestionsId.Count > 0)
            {
                var existingQuestionsList = await _db.Questions.Where(q => request.ExistingQuestionsId.Contains(q.Id)).ToListAsync();
                foreach (var existingQuestion in existingQuestionsList)
                {
                    quiz.QuizQuestions.Add(new QuizQuestion { QuestionId = existingQuestion.Id});
                }
            }
            if (request.Questions != null && request.Questions.Count > 0)
            {
                foreach (var question in request.Questions)
                {
                    var q = new Question { Text = question.Text, Answer = question.Answer };
                    quiz.QuizQuestions.Add(new QuizQuestion { Question = q});
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

        [HttpGet("export/formats")]
        public ActionResult<List<string>> GetExportFormats()
        {
            var formats = _exportService.GetFormats().ToList();
            return Ok(formats);
        }

        [HttpGet("{id:guid}/export")]
        public async Task<IActionResult> Export(Guid id, [FromQuery] string format)
        {

            var quiz = await _db.Quizzes
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                return NotFound();

            var exporter = _exportService.GetExporter(format);
            if (exporter == null)
                return BadRequest($"Unknown export format: {format}");

            var bytes = exporter.Export(quiz);

            var safeTitle = string.Join("_", quiz.Title.Split(Path.GetInvalidFileNameChars()));
            if (string.IsNullOrWhiteSpace(safeTitle))
            {
                safeTitle = "quiz";
            }

            var fileName = $"{safeTitle}.{exporter.FileExtension}";

            return File(bytes, exporter.ContentType, fileName);
        }
    }
}