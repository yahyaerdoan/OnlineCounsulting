using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.Storage.Common;

namespace OnlineConsulting.Storage.Providers;

/// <summary>Talks to any S3-compatible backend (real AWS S3, Cloudflare R2, Backblaze B2) through the same AWS SDK client - they all implement the S3 API, so one implementation covers all three instead of one per vendor. ServiceUrl is what actually picks the backend.</summary>
public class S3CompatibleStorageService : IStorageService
{
    private readonly S3StorageOptions _options;
    private readonly AmazonS3Client _client;

    public S3CompatibleStorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value.S3;

        var config = new AmazonS3Config
        {
            ServiceURL = _options.ServiceUrl,
            AuthenticationRegion = _options.Region,
            // R2/B2 don't support AWS's virtual-hosted-style bucket addressing (bucket.host.com) -
            // path-style (host.com/bucket) works uniformly across all three backends.
            ForcePathStyle = true,
        };
        _client = new AmazonS3Client(new BasicAWSCredentials(_options.AccessKey, _options.SecretKey), config);
    }

    public string ProviderName => StorageProviderNames.S3;

    public async Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        var (width, height) = await ImageDimensionReader.TryReadAsync(buffer, contentType, cancellationToken);

        var storedFileName = SafeFileNaming.GenerateStoredFileName(fileName);

        await _client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = storedFileName,
            InputStream = buffer,
            ContentType = contentType,
            AutoCloseStream = false,
        }, cancellationToken);

        var url = $"{_options.PublicBaseUrl.TrimEnd('/')}/{storedFileName}";

        return new UploadResult(url, buffer.Length, width, height);
    }

    public async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var key = Path.GetFileName(url);

        await _client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
        }, cancellationToken);
    }
}
