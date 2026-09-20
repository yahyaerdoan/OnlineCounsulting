using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.Storage.Common;

namespace OnlineConsulting.Storage.Providers;

/// <summary>Google Cloud Storage - not Google Drive, which was rejected: it isn't built for public hotlinking and rate-limits that pattern.</summary>
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

    public async Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();

        await fileStream.CopyToAsync(buffer, cancellationToken);

        buffer.Position = 0;

        var (width, height) = await ImageDimensionReader.TryReadAsync(buffer, contentType, cancellationToken);

        var storageKey = await SafeFileNaming.ResolveUniqueKeyAsync(folder, fileName, key => ExistsAsync(key, cancellationToken));

        _ = await _client.UploadObjectAsync(_options.BucketName, storageKey, contentType, buffer, cancellationToken: cancellationToken);

        var url = $"{BaseUrl}/{storageKey}";

        return new UploadResult(url, buffer.Length, width, height);
    }

    public async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var objectName = ToRelativeKey(url);

        await _client.DeleteObjectAsync(_options.BucketName, objectName, cancellationToken: cancellationToken);
    }

    private string BaseUrl => string.IsNullOrEmpty(_options.PublicBaseUrl) ? $"https://storage.googleapis.com/{_options.BucketName}" : _options.PublicBaseUrl.TrimEnd('/');

    /// <summary>Strips the base URL prefix to recover the storage key; falls back to the bare file name for assets uploaded before folders existed.</summary>
    private string ToRelativeKey(string url)
    {
        var prefix = BaseUrl + "/";

        return url.StartsWith(prefix, StringComparison.Ordinal) ? url[prefix.Length..] : Path.GetFileName(url);
    }

    private async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            _ = await _client.GetObjectAsync(_options.BucketName, key, cancellationToken: cancellationToken);

            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
