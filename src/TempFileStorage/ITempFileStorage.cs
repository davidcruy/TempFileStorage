using System;
using System.IO;
using System.Threading.Tasks;

namespace TempFileStorage
{
    public interface ITempFileStorage
    {
        Task<string> StoreFile(string filename, Stream contentStream);
        Task<string> StoreFile(string filename, Stream contentStream, TimeSpan timeout);
        Task<string> StoreFile(string filename, byte[] content);
        Task<string> StoreFile(string filename, byte[] content, TimeSpan timeout);

        Task<TempFile> GetStoredFile(string key);
        Task<bool> TryGetFile(string key, out string filename, out byte[] content);
    }
}
