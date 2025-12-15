using QuizAPI.Domain;
using System.ComponentModel.Composition;
using System.Text;

namespace QuizAPI.Export
{
    [Export(typeof(IQuizExporter))]
    public class CsvQuizExporter : IQuizExporter
    {
        public string Format => "csv";

        public string ContentType => "text/csv";

        public string FileExtension => "csv";

        public byte[] Export(Quiz quiz)
        {
            var sb = new StringBuilder();

            var questions = quiz.QuizQuestions.ToList();

            var index = 1;

            foreach (var qq in questions)
            {
                var text = qq.Question.Text ?? string.Empty;

                text = text.Replace("\"", "\"\"");

                sb.Append($"{index}. \"{text}\"");
                sb.AppendLine();

                index++;
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
