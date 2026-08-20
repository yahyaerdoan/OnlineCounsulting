using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.Storage.Common;

namespace OnlineConsulting.Storage.Providers;

public class LocalFileSystemStorageService(IOptions<StorageOptions> options) : IStorageService
{
    private readonly LocalStorageOptions _options = options.Value.Local;

    public string ProviderName => StorageProviderNames.Local;

    public async Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        // Buffered once so the same bytes can both be inspected for image dimensions and written to
        // disk, regardless of whether the caller's stream supports seeking.
        using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        var (width, height) = await ImageDimensionReader.TryReadAsync(buffer, contentType, cancellationToken);

        var storedFileName = SafeFileNaming.GenerateStoredFileName(fileName);

        _ = Directory.CreateDirectory(_options.RootPath);
        var fullPath = Path.Combine(_options.RootPath, storedFileName);

        await using (var fileOnDisk = File.Create(fullPath))
        {
            await buffer.CopyToAsync(fileOnDisk, cancellationToken);
        }

        var url = $"{_options.PublicPathPrefix.TrimEnd('/')}/{storedFileName}";

        return new UploadResult(url, buffer.Length, width, height);
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(url);
        var fullPath = Path.Combine(_options.RootPath, fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
