using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TempFileStorage
{
    public class TempFileMemoryStorage : ITempFileStorage
    {
        private readonly IDictionary<string, (TempFile FileInfo, byte[] Content)> _files;

        public TempFileMemoryStorage()
        {
            _files = new Dictionary<string, (TempFile, byte[])>();
        }

        public Task<TempFile> StoreFile(string filename, Stream contentStream) => StoreFile(filename, contentStream, TimeSpan.FromMinutes(30));

        public async Task<TempFile> StoreFile(string filename, Stream contentStream, TimeSpan timeout)
        {
            var memStream = new MemoryStream();
            await contentStream.CopyToAsync(memStream);

            var content = memStream.ToArray();
            var fileSize = content.Length;

            var file = new TempFile
            {
                Filename = filename,
                FileSize = fileSize,
                CacheTimeout = DateTime.Now.Add(timeout)
            };

            _files.Add(file.Key, (file, content));

            return file;
        }

        public Task<bool> ContainsKey(string key) => Task.FromResult(_files.ContainsKey(key));

        public Task<TempFile> GetFileInfo(string key)
        {
            var fileInfo = _files.TryGetValue(key, out var storedFile)
                ? storedFile.FileInfo
                : null;

            return Task.FromResult(fileInfo);
        }

        public Task<byte[]> Download(string key)
        {
            return Task.FromResult(_files[key].Content);
        }
    }
}