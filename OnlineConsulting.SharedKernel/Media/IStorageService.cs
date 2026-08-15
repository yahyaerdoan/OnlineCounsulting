namespace OnlineConsulting.SharedKernel.Media;

/// <summary>One implementation per backend (Local/AzureBlob/S3, mirrors IPaymentGateway's provider pattern). Callers depend on this interface only, so swapping the active backend is a config change (Storage:ActiveProvider), not a code change.</summary>
public interface IStorageService
{
    /// <summary>Matches one of StorageProviderNames - also the keyed-DI service key this implementation is registered under.</summary>
    string ProviderName { get; }

    Task<UploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);

    Task DeleteAsync(string url, CancellationToken cancellationToken = default);
}

/// <summary>Width/Height are null for non-image content (or when the backend doesn't inspect the file) - never assume they're populated.</summary>
public record UploadResult(string Url, long SizeBytes, int? Width, int? Height);
