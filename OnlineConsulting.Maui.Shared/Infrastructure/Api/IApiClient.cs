using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Thin typed HttpClient wrapper for calling the Api, shared by every module.</summary>
public interface IApiClient
{
    /// <summary>Origin this client sends requests to - may be a service-discovery address only the server can resolve.</summary>
    Uri? BaseAddress { get; }

    /// <summary>Origin a browser or BlazorWebView loads Api-served media from - used to turn a storage-relative media Url (e.g. "/media/x.jpg") into one the page can load.</summary>
    Uri? PublicBaseAddress { get; }

    Task<ApiEnvelope<T>> GetAsync<T>(string path, CancellationToken cancellationToken = default);

    /// <summary>POST-based search/filter for DynamicQuery-driven lists - see ServerDataTable.</summary>
    Task<ApiEnvelope<T>> QueryAsync<T>(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope<T>> PostAsync<T>(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> PostAsync(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> PutAsync(string path, object? body, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>For multipart/file-upload endpoints.</summary>
    Task<ApiEnvelope<T>> PostFileAsync<T>(string path, MultipartFormDataContent content, CancellationToken cancellationToken = default);

    /// <summary>Calls an action the Api offered in "_links", with the link's own method; body is ignored for GET and DELETE.</summary>
    Task<ApiEnvelope<T>> FollowAsync<T>(HalLink link, object? body = null, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="FollowAsync{T}(HalLink, object?, CancellationToken)"/>
    Task<ApiEnvelope> FollowAsync(HalLink link, object? body = null, CancellationToken cancellationToken = default);
}
