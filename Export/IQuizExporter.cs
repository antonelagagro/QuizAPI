using QuizAPI.Domain;

namespace QuizAPI.Export
{
    public interface IQuizExporter
    {
        string Format { get; }
        string ContentType { get; }
        string FileExtension { get; }
        byte[] Export(Quiz quiz);
    }
}
