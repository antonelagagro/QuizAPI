namespace QuizAPI.Domain
{
    public class Quiz
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Title { get; set; }

        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
    }
}
