using System;
using System.IO;

namespace TempFileStorage
{
    public interface ITempFileStorage
    {
        string StoreFile(string filename, Stream contentStream);
        string StoreFile(string filename, Stream contentStream, TimeSpan timeout);
        string StoreFile(string filename, byte[] content);
        string StoreFile(string filename, byte[] content, TimeSpan timeout);

        TempFile GetStoredFile(string key);
        bool TryGetFile(string key, out string filename, out byte[] content);
    }
}
