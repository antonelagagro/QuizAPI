namespace QuizAPI.Domain
{
    public class QuizQuestion
    {
        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;


    }
}
