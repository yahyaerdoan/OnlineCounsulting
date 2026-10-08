using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;
using Polly.CircuitBreaker;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Optional deps are null for the anonymous pre-auth client (Login, TokenRefresher).</summary>
public class ApiClient(HttpClient httpClient, IAccessTokenProvider? tokenProvider = null, TokenRefresher? tokenRefresher = null, AuthenticationExpiredNotifier? expiredNotifier = null, PublicApiOrigin? publicOrigin = null) : IApiClient
{
    private const string NetworkErrorMessage = "Could not reach the server. Check your connection and try again.";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Uri? BaseAddress => httpClient.BaseAddress;

    public Uri? PublicBaseAddress => publicOrigin?.BaseAddress ?? httpClient.BaseAddress;

    public Task<ApiEnvelope<T>> GetAsync<T>(string path, CancellationToken cancellationToken = default) =>
        SendAsync<T>(HttpMethod.Get, path, null, cancellationToken);

    /// <summary>POST to a /query sub-resource - Swagger can't document HTTP QUERY.</summary>
    public Task<ApiEnvelope<T>> QueryAsync<T>(string path, object? body, CancellationToken cancellationToken = default) =>
        SendAsync<T>(HttpMethod.Post, path, JsonContent.Create(body, options: JsonOptions), cancellationToken);

    public Task<ApiEnvelope<T>> PostAsync<T>(string path, object? body, CancellationToken cancellationToken = default) =>
        SendAsync<T>(HttpMethod.Post, path, JsonContent.Create(body, options: JsonOptions), cancellationToken);

    public Task<ApiEnvelope> PostAsync(string path, object? body, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, path, JsonContent.Create(body, options: JsonOptions), cancellationToken);

    public Task<ApiEnvelope> PutAsync(string path, object? body, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, path, JsonContent.Create(body, options: JsonOptions), cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(string path, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, path, null, cancellationToken);

    public Task<ApiEnvelope<T>> PostFileAsync<T>(string path, MultipartFormDataContent content, CancellationToken cancellationToken = default) =>
        SendAsync<T>(HttpMethod.Post, path, content, cancellationToken);

    public Task<ApiEnvelope<T>> FollowAsync<T>(HalLink link, object? body = null, CancellationToken cancellationToken = default) =>
        SendAsync<T>(link.HttpMethod, link.RelativePath, ContentFor(link, body), cancellationToken);

    public Task<ApiEnvelope> FollowAsync(HalLink link, object? body = null, CancellationToken cancellationToken = default) =>
        SendAsync(link.HttpMethod, link.RelativePath, ContentFor(link, body), cancellationToken);

    private static JsonContent? ContentFor(HalLink link, object? body) =>
        link.HttpMethod == HttpMethod.Get || link.HttpMethod == HttpMethod.Delete ? null : JsonContent.Create(body, options: JsonOptions);

    private async Task<ApiEnvelope<T>> SendAsync<T>(HttpMethod method, string path, HttpContent? content, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await SendCoreAsync(method, path, content, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or BrokenCircuitException)
        {
            return new ApiEnvelope<T>(default, false, 0, NetworkErrorMessage, null);
        }

        using (response)
        {
            NotifyIfExpired(response);
            return await ReadEnvelopeAsync<T>(response, cancellationToken);
        }
    }

    private async Task<ApiEnvelope> SendAsync(HttpMethod method, string path, HttpContent? content, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await SendCoreAsync(method, path, content, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or BrokenCircuitException)
        {
            return new ApiEnvelope(false, 0, NetworkErrorMessage, null);
        }

        using (response)
        {
            NotifyIfExpired(response);
            return await ReadEnvelopeAsync(response, cancellationToken);
        }
    }

    private async Task<HttpResponseMessage> SendCoreAsync(HttpMethod method, string path, HttpContent? content, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path) { Content = content };

        if (tokenProvider is not null)
        {
            var accessToken = tokenRefresher is not null
                ? await tokenRefresher.GetAccessTokenAsync(cancellationToken)
                : (await tokenProvider.GetTokenSetAsync())?.AccessToken;

            if (accessToken is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        return await httpClient.SendAsync(request, cancellationToken);
    }

    private void NotifyIfExpired(HttpResponseMessage response)
    {
        if (expiredNotifier is not null && response.StatusCode == HttpStatusCode.Unauthorized)
        {
            expiredNotifier.NotifyExpired();
        }
    }

    private static async Task<ApiEnvelope<T>> ReadEnvelopeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!HasBody(response))
        {
            return new ApiEnvelope<T>(default, response.IsSuccessStatusCode, (int)response.StatusCode, response.IsSuccessStatusCode ? response.ReasonPhrase : null, null);
        }

        if (response.IsSuccessStatusCode)
        {
            var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<T>>(JsonOptions, cancellationToken);
            return envelope ?? new ApiEnvelope<T>(default, false, (int)response.StatusCode, response.ReasonPhrase, null);
        }

        var (Message, Errors, FieldErrors) = await ReadProblemDetailsAsync(response, cancellationToken);
        return new ApiEnvelope<T>(default, false, (int)response.StatusCode, Message, Errors, FieldErrors);
    }

    private static async Task<ApiEnvelope> ReadEnvelopeAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!HasBody(response))
        {
            return new ApiEnvelope(response.IsSuccessStatusCode, (int)response.StatusCode, response.IsSuccessStatusCode ? response.ReasonPhrase : null, null);
        }

        if (response.IsSuccessStatusCode)
        {
            var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope>(JsonOptions, cancellationToken);
            return envelope ?? new ApiEnvelope(false, (int)response.StatusCode, response.ReasonPhrase, null);
        }

        var (Message, Errors, FieldErrors) = await ReadProblemDetailsAsync(response, cancellationToken);
        return new ApiEnvelope(false, (int)response.StatusCode, Message, Errors, FieldErrors);
    }

    private static bool HasBody(HttpResponseMessage response)
        => response.StatusCode != HttpStatusCode.NoContent && response.Content.Headers.ContentLength is not 0;

    private static async Task<(string? Message, List<string>? Errors, Dictionary<string, List<string>>? FieldErrors)> ReadProblemDetailsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsPayload>(JsonOptions, cancellationToken);
        return (problem?.Detail ?? problem?.Title, problem?.Errors, problem?.FieldErrors);
    }

    private sealed record ProblemDetailsPayload(string? Type, string? Title, int? Status, string? Detail, string? Instance, List<string>? Errors, Dictionary<string, List<string>>? FieldErrors);
}
