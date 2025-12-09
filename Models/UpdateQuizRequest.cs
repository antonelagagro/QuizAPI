namespace QuizAPI.Models
{
    public class UpdateQuizRequest
    {
        public string? Title { get; set; }
        public List<CreateQuestionRequest> Questions { get; set; } = [];
    }
}
