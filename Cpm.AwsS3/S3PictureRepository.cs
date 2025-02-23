using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Cpm.Core;
using Cpm.Core.Extensions;
using Cpm.Core.Models;
using Cpm.Core.Services;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Notes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Optional;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Cpm.AwsS3
{
    public class S3PictureRepository : IPictureRepository
    {
        private readonly IAuditDataProvider _auditDataProvider;
        private readonly IPictureMetadataSerializer _serializer;
        private readonly IFieldRepository _fieldRepository;
        private readonly ILogger<S3PictureRepository> _logger;
        private readonly IAmazonS3 _client;
        private readonly string _bucketName;
        private readonly TimeSpan _downloadTimeout;
        private readonly TimeSpan _uploadTimeout;
        private readonly IImageEncoder _thumbnailEncoder;
        private readonly ResizeOptions _resizeOptions;
        private readonly List<UploadJob> _jobs;
        private Task _task;

        private const string ThumbnailExtension = ".jpeg";
        private const string ThumbnailPrefix = "thumb";
        private const string FullSizePrefix = "full";

        private readonly int _idleSleep;
        private readonly int _retryWait;
        private readonly int _maxRetry;

        public S3PictureRepository(
            IOptions<S3PictureRepoOptions> options,
            IAuditDataProvider auditDataProvider,
            IPictureMetadataSerializer serializer,
            IFieldRepository fieldRepository,
            ILogger<S3PictureRepository> logger,
            AWSCredentials awsCredentials
            )
        {
            _auditDataProvider = auditDataProvider;
            _serializer = serializer;
            _fieldRepository = fieldRepository;
            _logger = logger;
            _bucketName = options.Value.BucketName;
            
            _downloadTimeout = options.Value.DownloadTimeout;
            _uploadTimeout = options.Value.UploadTimeout;
            _idleSleep = options.Value.UploadIdleSleepMs;
            _retryWait = options.Value.UploadRetryWaitMs;
            _maxRetry = options.Value.UploadMaxRetry;

            _client = new AmazonS3Client(
                awsCredentials,
                RegionEndpoint.GetBySystemName(options.Value.Region)
                );

            _thumbnailEncoder = new JpegEncoder
            {
                Quality = options.Value.ThumbnailQuality,
            };

            _resizeOptions = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(
                    options.Value.ThumbnailWidth, 
                    options.Value.ThumbnailHeight
                    ),
            };

            _jobs = new List<UploadJob>();

            _task = Task.Run(ProcessQueue);
        }

        private async Task ProcessQueue()
        {
            while (true)
            {
                var jobsToInitiate = _jobs
                    .ToArray()
                    .Where(x => x.LastAttempt
                        .ToOption()
                        .Map(la => Clock.Now.Subtract(la).TotalMilliseconds >= _retryWait)
                        .ValueOr(true)
                    )
                    .ToArray();

                if (jobsToInitiate.Any())
                {
                    _logger.LogInformation("Received {jobs} new jobs", jobsToInitiate.Length);
                }

                var tasks = jobsToInitiate
                    .Select(j =>
                    {
                        j.LastAttempt = Clock.Now;
                        ++j.Attempts;
                        return Upload(j.Key, j.Stream);
                    })
                    .ToArray();

                await (tasks.Any() ? Task.WhenAll(tasks) : Task.Delay(_idleSleep));

                jobsToInitiate
                    .Zip(tasks, (job, task) => new {job, task})
                    .ForEach(x =>
                    {
                        if (x.task.IsCompletedSuccessfully)
                        {
                            _jobs.Remove(x.job);
                            _logger.LogInformation("Completed the job for {key}", x.job.Key);
                        }
                        else
                        {
                            _logger.LogInformation("Job for {key} failed at {attempt} attempt", x.job.Key, x.job.Attempts);
                            if (x.job.Attempts >= _maxRetry)
                            {
                                _jobs.Remove(x.job);
                            }
                        }
                    });
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public async Task Store(string siteId, string fieldId, DateTime date, string tag, Stream stream,
            string originalFileName)
        {
            var id = Guid.NewGuid().ToString("D");

            _logger.LogDebug("Storing picture - start ({originalName})", originalFileName);

            var originalExtension = Path.GetExtension(originalFileName);

            var md = new PictureMetadata
            {
                Id = id,
                UploadedBy = _auditDataProvider.GetUserField(),
                OriginalName = Path.GetFileNameWithoutExtension(originalFileName),
                OriginalExtension = originalExtension,
            };

            using (var image = Image.Load(stream))
            {
                _logger.LogTrace("Storing picture - loaded ({originalName})", originalFileName);
                ExifInterpreter.PopulateMetadata(md, image.MetaData.ExifProfile);
                _logger.LogTrace("Storing picture - metadata read ({originalName})", originalFileName);

                await _fieldRepository.SaveFieldNote(fieldId, date, n =>
                {
                    if (n.IsDeleted)
                    {
                        n.SerializedPictureMetadata = null;
                        n.Text = null;
                        n.IsDeleted = false;
                    }
                    AddPictureToNote(md, n);
                });
                _logger.LogTrace("Storing picture - note updated ({originalName})", originalFileName);

                stream.Seek(0, SeekOrigin.Begin);
                QueueUpload(GetKeyFor(siteId, fieldId, date, id, FullSizePrefix, originalExtension), stream);
                _logger.LogTrace("Storing picture - full size upload queued ({originalName})", originalFileName);

                image.Mutate(x => x.Resize(_resizeOptions));

                using (var thumbnailStream = new MemoryStream())
                {
                    image.Save(thumbnailStream, _thumbnailEncoder);
                    _logger.LogTrace("Storing picture - thumbnail created ({originalName})", originalFileName);

                    thumbnailStream.Seek(0, SeekOrigin.Begin);
                    QueueUpload(GetKeyFor(siteId, fieldId, date, id, ThumbnailPrefix, ThumbnailExtension), thumbnailStream);
                }
                _logger.LogTrace("Storing picture - thumbnail upload queued and done. ({originalName})", originalFileName);
            }
        }

        public string GetFullUrl(string siteId, string fieldId, DateTime date, string id, string fileExtension)
            => GetUrl(siteId, fieldId, date, id, FullSizePrefix, fileExtension);

        public string GetThumbUrl(string siteId, string fieldId, DateTime date, string id)
            => GetUrl(siteId, fieldId, date, id, ThumbnailPrefix, ThumbnailExtension);

        public Task DeletePicture(string fieldId, DateTime date, string id)
        {
            return _fieldRepository.SaveFieldNote(fieldId, date, n => RemovePictureFromNote(id, n));
        }

        private void QueueUpload(string key, Stream stream)
        {
            _jobs.Add(new UploadJob(key, stream));
        }

        private void AddPictureToNote(PictureMetadata metadata, PinnedNote note)
        {
            note.SerializedPictureMetadata = _serializer.Serialize(
                _serializer.Deserialize(note.SerializedPictureMetadata)
                    .Append(metadata)
                );
        }

        private void RemovePictureFromNote(string id, PinnedNote note)
        {
            note.SerializedPictureMetadata = _serializer.Serialize(
                _serializer.Deserialize(note.SerializedPictureMetadata)
                    .Where(x => x.Id != id)
                );
        }

        private Task Upload(string key, Stream stream)
        {
            var request = new PutObjectRequest
            {
                Key = key,
                BucketName = _bucketName,
                InputStream = stream,
            };

            return _client.PutObjectAsync(request);
        }

        private string GetKeyFor(string siteId, string fieldId, DateTime date, string id, string type,
            string fileExtension)
            => $"{siteId}/{fieldId}/{date:yyyy-MM-dd}/{type}-{id}{fileExtension}";

        private string GetUrl(string siteId, string fieldId, DateTime date, string id, string type,
            string fileExtension)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = GetKeyFor(siteId, fieldId, date, id, type, fileExtension),
                Expires = DateTime.Now.Add(_downloadTimeout)
            };

            return _client.GetPreSignedURL(request);
        }
    }
}
