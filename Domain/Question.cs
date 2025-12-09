namespace QuizAPI.Domain
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Text { get; set; }
        public required string Answer { get; set; }

        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
    }
}
