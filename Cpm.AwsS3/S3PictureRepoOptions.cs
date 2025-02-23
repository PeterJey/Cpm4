using System;

namespace Cpm.AwsS3
{
    public class S3PictureRepoOptions
    {
        public string Region { get; set; }
        public string BucketName { get; set; }
        public TimeSpan DownloadTimeout { get; set; }
        public TimeSpan UploadTimeout { get; set; }
        public int ThumbnailQuality { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int UploadIdleSleepMs { get; set; }
        public int UploadRetryWaitMs { get; set; }
        public int UploadMaxRetry { get; set; }
    }
}