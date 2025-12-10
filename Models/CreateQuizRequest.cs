namespace QuizAPI.Models
{
    public class CreateQuizRequest
    {
        public required string Title { get; set; }
        public List<Guid> ExistingQuestionsId { get; set; } = [];
        public List<CreateQuestionRequest> Questions { get; set; } = [];
    }
}
