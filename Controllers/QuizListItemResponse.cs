
namespace QuizAPI.Controllers
{
    internal class QuizListItemResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
    }
}