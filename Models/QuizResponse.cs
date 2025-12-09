namespace QuizAPI.Models
{
    public class QuizResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
    }
}
