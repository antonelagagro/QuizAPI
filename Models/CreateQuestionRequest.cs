namespace QuizAPI.Models
{
    public class CreateQuestionRequest
    {
        public required string Text { get; set; }
        public required string Answer { get; set; }

    }
}
