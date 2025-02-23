using System;
using System.IO;

namespace Cpm.AwsS3
{
    public class UploadJob
    {
        public string Key { get; }
        public Stream Stream { get; }

        public UploadJob(string key, Stream stream)
        {
            Key = key;
            Stream = new MemoryStream();
            stream.CopyTo(Stream);
            Stream.Seek(0, SeekOrigin.Begin);
            Attempts = 0;
        }

        public int Attempts { get; set; }

        public DateTime? LastAttempt { get; set; }
    }
}