using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.Storage.Common;

namespace OnlineConsulting.Storage.Providers;

/// <summary>Google Cloud Storage (a real object-storage service with a free tier) - not to be confused with Google Drive, which was considered and rejected: Drive isn't built for public hotlinking and rate-limits/blocks exactly this usage pattern.</summary>
public class GoogleCloudStorageService : IStorageService
{
    private readonly GoogleCloudStorageOptions _options;
    private readonly StorageClient _client;

    public GoogleCloudStorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value.GoogleCloud;
        var credential = CredentialFactory.FromJson<ServiceAccountCredential>(_options.CredentialsJson).ToGoogleCredential();
        _client = StorageClient.Create(credential);
    }

    public string ProviderName => StorageProviderNames.GoogleCloud;

    public async Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        var (width, height) = await ImageDimensionReader.TryReadAsync(buffer, contentType, cancellationToken);

        var storedFileName = SafeFileNaming.GenerateStoredFileName(fileName);

        await _client.UploadObjectAsync(_options.BucketName, storedFileName, contentType, buffer, cancellationToken: cancellationToken);

        var baseUrl = string.IsNullOrEmpty(_options.PublicBaseUrl)
            ? $"https://storage.googleapis.com/{_options.BucketName}"
            : _options.PublicBaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/{storedFileName}";

        return new UploadResult(url, buffer.Length, width, height);
    }

    public async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var objectName = Path.GetFileName(url);
        await _client.DeleteObjectAsync(_options.BucketName, objectName, cancellationToken: cancellationToken);
    }
}
