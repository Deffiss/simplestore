using System.Threading.Tasks;

namespace StoreSample.Commands.Services.Text
{
    public class SimpleHtmlSanitizer : IHtmlSanitizer
    {
        public Task<string> Sanitize(string html) => throw new System.NotImplementedException();
    }
}
