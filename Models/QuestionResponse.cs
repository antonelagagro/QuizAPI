namespace QuizAPI.Models
{
    public class QuestionResponse
    {
        public Guid Id { get; set; }
        public required string Text { get; set; }
    }
}
