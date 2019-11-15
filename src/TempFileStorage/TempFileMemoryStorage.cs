using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TempFileStorage
{
    public class TempFileMemoryStorage : ITempFileStorage
    {
        private readonly IDictionary<string, TempFile> _files;

        public TempFileMemoryStorage()
        {
            _files = new Dictionary<string, TempFile>();
        }

        public string StoreFile(string filename, Stream contentStream) => StoreFile(filename, contentStream, TimeSpan.FromMinutes(30));

        public string StoreFile(string filename, Stream contentStream, TimeSpan timeout)
        {
            var memStream = new MemoryStream();
            contentStream.Seek(0, SeekOrigin.Begin);
            contentStream.CopyTo(memStream);

            var content = memStream.ToArray();

            return StoreFile(filename, content, timeout);
        }

        public string StoreFile(string filename, byte[] content) => StoreFile(filename, content, TimeSpan.FromMinutes(30));

        public string StoreFile(string filename, byte[] content, TimeSpan timeout)
        {
            var file = new TempFile
            {
                Filename = filename,
                CacheTimeout = DateTime.Now.Add(timeout),
                Content = content
            };

            _files.Add(file.Key, file);

            return file.Key;
        }

        public bool TryGetFile(string key, out string filename, out byte[] content)
        {
            if (_files.TryGetValue(key, out var tempFile))
            {
                filename = tempFile.Filename;
                content = tempFile.Content;

                return true;
            }

            filename = null;
            content = null;

            return false;
        }

        public TempFile GetStoredFile(string key)
        {
            var theFile = _files.FirstOrDefault(x => x.Key == key).Value;

            return theFile;
        }
    }
}