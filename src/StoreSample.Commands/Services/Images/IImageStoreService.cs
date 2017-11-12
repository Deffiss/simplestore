using System.IO;
using System.Threading.Tasks;

namespace StoreSample.Commands.Services.Images
{
    public interface IImageStoreService
    {
        Task<string> SaveImage(Stream imageStream, string category, string name);
    }
}
