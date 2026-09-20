using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.Storage.Common;

namespace OnlineConsulting.Storage.Providers;

public class LocalFileSystemStorageService(IOptions<StorageOptions> options) : IStorageService
{
    private readonly LocalStorageOptions _options = options.Value.Local;

    public string ProviderName => StorageProviderNames.Local;

    /// <summary>Buffers the stream once so the same bytes can be inspected for image dimensions and written to disk, regardless of whether the caller's stream supports seeking.</summary>
    public async Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();

        await fileStream.CopyToAsync(buffer, cancellationToken);

        buffer.Position = 0;

        var (width, height) = await ImageDimensionReader.TryReadAsync(buffer, contentType, cancellationToken);

        var storageKey = await SafeFileNaming.ResolveUniqueKeyAsync(folder, fileName, key => Task.FromResult(File.Exists(Path.Combine(_options.RootPath, key.Replace('/', Path.DirectorySeparatorChar)))));

        var fullPath = Path.Combine(_options.RootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        _ = Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using (var fileOnDisk = File.Create(fullPath))
        {
            await buffer.CopyToAsync(fileOnDisk, cancellationToken);
        }

        var url = $"{_options.PublicPathPrefix.TrimEnd('/')}/{storageKey}";

        return new UploadResult(url, buffer.Length, width, height);
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var relativeKey = ToRelativeKey(url);
        var fullPath = Path.Combine(_options.RootPath, relativeKey.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    /// <summary>Strips the public path prefix to recover the storage key; falls back to the bare file name for assets uploaded before folders existed.</summary>
    private string ToRelativeKey(string url)
    {
        var prefix = _options.PublicPathPrefix.TrimEnd('/') + "/";

        return url.StartsWith(prefix, StringComparison.Ordinal) ? url[prefix.Length..] : Path.GetFileName(url);
    }
}
