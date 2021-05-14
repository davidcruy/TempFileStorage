using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TempFileStorage
{
    public class TempFileMemoryStorage : ITempFileStorage
    {
        private readonly IDictionary<string, TempFile> _files;

        public TempFileMemoryStorage()
        {
            _files = new Dictionary<string, TempFile>();
        }

        public Task<string> StoreFile(string filename, Stream contentStream) => StoreFile(filename, contentStream, TimeSpan.FromMinutes(30));

        public Task<string> StoreFile(string filename, Stream contentStream, TimeSpan timeout)
        {
            var memStream = new MemoryStream();
            contentStream.Seek(0, SeekOrigin.Begin);
            contentStream.CopyTo(memStream);

            var content = memStream.ToArray();

            return StoreFile(filename, content, timeout);
        }

        public Task<string> StoreFile(string filename, byte[] content) => StoreFile(filename, content, TimeSpan.FromMinutes(30));

        public Task<string> StoreFile(string filename, byte[] content, TimeSpan timeout)
        {
            var file = new TempFile
            {
                Filename = filename,
                CacheTimeout = DateTime.Now.Add(timeout),
                Content = content
            };

            _files.Add(file.Key, file);

            return Task.FromResult(file.Key);
        }

        public Task<bool> TryGetFile(string key, out string filename, out byte[] content)
        {
            if (_files.TryGetValue(key, out var tempFile))
            {
                filename = tempFile.Filename;
                content = tempFile.Content;

                return Task.FromResult(true);
            }

            filename = null;
            content = null;

            return Task.FromResult(false);
        }

        public Task<TempFile> GetStoredFile(string key)
        {
            var theFile = _files.FirstOrDefault(x => x.Key == key).Value;

            return Task.FromResult(theFile);
        }
    }
}