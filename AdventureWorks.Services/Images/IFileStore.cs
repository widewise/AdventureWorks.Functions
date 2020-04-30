using System.IO;
using System.Threading.Tasks;

namespace AdventureWorks.Services.Images
{
    public interface IFileStore
    {
        Task<string> Save(string fileName, Stream fileStream);
    }
}
