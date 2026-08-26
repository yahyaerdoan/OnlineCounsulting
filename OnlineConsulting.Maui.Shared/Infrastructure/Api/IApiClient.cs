namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Thin typed HttpClient wrapper for calling OnlineConsulting.Api from the admin UI (Web host and MAUI head alike) - one client for every module rather than one per module, since the envelope shape and auth handling are identical everywhere.</summary>
public interface IApiClient
{
    Task<ApiEnvelope<T>> GetAsync<T>(string path, CancellationToken cancellationToken = default);

    /// <summary>POST-based search/filter for DynamicQuery-driven lists - see ServerDataTable.</summary>
    Task<ApiEnvelope<T>> QueryAsync<T>(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope<T>> PostAsync<T>(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> PostAsync(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> PutAsync(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>For multipart endpoints (e.g. Media's UploadMediaAsset) - the caller builds the MultipartFormDataContent so this stays generic across every file-accepting endpoint rather than special-casing one.</summary>
    Task<ApiEnvelope<T>> PostFileAsync<T>(string path, MultipartFormDataContent content, CancellationToken cancellationToken = default);
}
