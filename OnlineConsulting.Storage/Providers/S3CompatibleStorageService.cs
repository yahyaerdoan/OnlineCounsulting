using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.Storage.Common;

namespace OnlineConsulting.Storage.Providers;

/// <summary>Talks to any S3-compatible backend (AWS S3, Cloudflare R2, Backblaze B2) via the AWS SDK - ServiceUrl picks the actual backend.</summary>
public class S3CompatibleStorageService : IStorageService
{
    private readonly S3StorageOptions _options;
    private readonly AmazonS3Client _client;

    /// <summary>Uses path-style bucket addressing (host.com/bucket) rather than AWS's virtual-hosted style, since R2/B2 don't support the latter and path-style works uniformly across all three backends.</summary>
    public S3CompatibleStorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value.S3;

        var config = new AmazonS3Config
        {
            ServiceURL = _options.ServiceUrl,
            AuthenticationRegion = _options.Region,
            ForcePathStyle = true,
        };

        _client = new AmazonS3Client(new BasicAWSCredentials(_options.AccessKey, _options.SecretKey), config);
    }

    public string ProviderName => StorageProviderNames.S3;

    public async Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();

        await fileStream.CopyToAsync(buffer, cancellationToken);

        buffer.Position = 0;

        var (width, height) = await ImageDimensionReader.TryReadAsync(buffer, contentType, cancellationToken);

        var storageKey = await SafeFileNaming.ResolveUniqueKeyAsync(folder, fileName, key => ExistsAsync(key, cancellationToken));

        _ = await _client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = storageKey,
            InputStream = buffer,
            ContentType = contentType,
            AutoCloseStream = false,
        }, cancellationToken);

        var url = $"{_options.PublicBaseUrl.TrimEnd('/')}/{storageKey}";

        return new UploadResult(url, buffer.Length, width, height);
    }

    public async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var key = ToRelativeKey(url);

        _ = await _client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
        }, cancellationToken);
    }

    /// <summary>Strips the public base URL prefix to recover the storage key; falls back to the bare file name for assets uploaded before folders existed.</summary>
    private string ToRelativeKey(string url)
    {
        var prefix = _options.PublicBaseUrl.TrimEnd('/') + "/";

        return url.StartsWith(prefix, StringComparison.Ordinal) ? url[prefix.Length..] : Path.GetFileName(url);
    }

    private async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            _ = await _client.GetObjectMetadataAsync(_options.BucketName, key, cancellationToken);

            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
