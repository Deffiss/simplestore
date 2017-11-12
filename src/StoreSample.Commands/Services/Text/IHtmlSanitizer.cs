using System.Threading.Tasks;

namespace StoreSample.Commands.Services.Text
{
    public interface IHtmlSanitizer
    {
        Task<string> Sanitize(string html);
    }
}
