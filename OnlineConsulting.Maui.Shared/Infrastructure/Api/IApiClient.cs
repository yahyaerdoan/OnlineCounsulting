namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Thin typed HttpClient wrapper for calling the Api, shared by every module.</summary>
public interface IApiClient
{
    Task<ApiEnvelope<T>> GetAsync<T>(string path, CancellationToken cancellationToken = default);

    /// <summary>POST-based search/filter for DynamicQuery-driven lists - see ServerDataTable.</summary>
    Task<ApiEnvelope<T>> QueryAsync<T>(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope<T>> PostAsync<T>(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> PostAsync(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> PutAsync(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>For multipart/file-upload endpoints.</summary>
    Task<ApiEnvelope<T>> PostFileAsync<T>(string path, MultipartFormDataContent content, CancellationToken cancellationToken = default);
}
