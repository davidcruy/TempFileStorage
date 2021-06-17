using System;
using System.IO;
using System.Threading.Tasks;

namespace TempFileStorage
{
    public interface ITempFileStorage
    {
        Task<TempFile> StoreFile(string filename, Stream contentStream);
        Task<TempFile> StoreFile(string filename, Stream contentStream, TimeSpan timeout);

        Task<bool> ContainsKey(string key);
        Task<TempFile> GetFileInfo(string key);
        Task<byte[]> Download(string key);
    }
}
