using System;

namespace TempFileStorage
{
    public class TempFile
    {
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private static readonly Random random = new Random(DateTime.Now.Millisecond);

        public TempFile()
        {
            var key = "";

            for (var i = 0; i < 10; i++)
            {
                var rnd = random.Next(0, Characters.Length);
                key += Characters[rnd];
            }

            Key = key;
        }

        public string Key { get; }
        public string Filename { get; set; }
        public byte[] Content { get; set; }
        public DateTime CacheTimeout { get; set; }
    }
}