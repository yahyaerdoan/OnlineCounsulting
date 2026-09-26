using System.Diagnostics.CodeAnalysis;

namespace OnlineConsulting.SharedKernel.Media;

/// <summary>One implementation per backend (Local/AzureBlob/S3); swapping the active one is a config change (Storage:ActiveProvider), not a code change.</summary>
public interface IStorageService
{
    /// <summary>Matches one of StorageProviderNames - also the keyed-DI service key this implementation is registered under.</summary>
    string ProviderName { get; }

    Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default);

    Task DeleteAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>The host-independent form of a Url this backend serves, for persisting: an absolute Url that points at this backend's own public path becomes relative, so a record saved from one host still loads from another (e.g. the Android emulator). Urls this backend does not own pass through unchanged.</summary>
    [return: NotNullIfNotNull(nameof(url))]
    string? ToStoredUrl(string? url);
}

/// <summary>Width/Height are null for non-image content (or when the backend doesn't inspect the file) - never assume they're populated.</summary>
public record UploadResult(string Url, long SizeBytes, int? Width, int? Height);
