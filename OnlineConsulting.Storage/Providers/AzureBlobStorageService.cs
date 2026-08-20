using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.Storage.Common;

namespace OnlineConsulting.Storage.Providers;

public class AzureBlobStorageService : IStorageService
{
    private readonly BlobContainerClient _container;

    public AzureBlobStorageService(IOptions<StorageOptions> options)
    {
        var azureOptions = options.Value.AzureBlob;
        _container = new BlobContainerClient(azureOptions.ConnectionString, azureOptions.ContainerName);
    }

    public string ProviderName => StorageProviderNames.AzureBlob;

    public async Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        var (width, height) = await ImageDimensionReader.TryReadAsync(buffer, contentType, cancellationToken);

        var storedFileName = SafeFileNaming.GenerateStoredFileName(fileName);
        var blobClient = _container.GetBlobClient(storedFileName);

        _ = await blobClient.UploadAsync(buffer, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        return new UploadResult(blobClient.Uri.ToString(), buffer.Length, width, height);
    }

    public async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var blobName = Path.GetFileName(url);
        _ = await _container.DeleteBlobIfExistsAsync(blobName, cancellationToken: cancellationToken);
    }
}
