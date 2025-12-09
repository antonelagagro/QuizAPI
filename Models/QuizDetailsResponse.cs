namespace QuizAPI.Models
{
    public class QuizDetailsResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public List<QuestionResponse> Questions { get; set; } = [];
    }
}
