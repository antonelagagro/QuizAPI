using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace QuizAPI.Export
{
    public class QuizExportService
    {
        private readonly CompositionContainer _container;

        [ImportMany]
        public IEnumerable<IQuizExporter> Exporters { get; set; } = [];

        public QuizExportService()
        {
            var catalog = new AssemblyCatalog(typeof(QuizExportService).Assembly);

            _container = new CompositionContainer(catalog);

            _container.ComposeParts(this);
        }

        public IQuizExporter? GetExporter(string format)
        {
            return Exporters.FirstOrDefault(e =>
                e.Format.Equals(format, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> GetFormats()
        {
            return Exporters.Select(e => e.Format);
        }
    }
}
