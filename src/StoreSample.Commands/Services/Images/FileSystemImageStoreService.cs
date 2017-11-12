using System.IO;
using System.Threading.Tasks;

namespace StoreSample.Commands.Services.Images
{
    public class FileSystemImageStoreService : IImageStoreService
    {
        public Task<string> SaveImage(Stream imageStream, string category, string name) => Task.FromResult($"{category}/{name}");
    }
}
