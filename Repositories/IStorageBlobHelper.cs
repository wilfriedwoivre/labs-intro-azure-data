using System.Collections.Generic;
using System.Threading.Tasks;
using Soat.Masterclass.Labs.Models.Storage;

namespace Soat.Masterclass.Labs.Repositories
{
    public interface IStorageBlobHelper
    {
        Task CreateBlobAsync(string fileName, string content);
        Task UpdateBlobAsync(string fileName, string content);
        Task DeleteBlobAsync(string fileName);
        Task<IEnumerable<BlobItem>> ListBlobAsync();
        Task<BlobItem> GetBlobContentAsync(string fileName);
    }
}