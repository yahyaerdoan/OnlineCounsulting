using System.Text.Json;

namespace OnlineConsulting.UserInterface.Infrastructure.Api;

public class ApiClient(HttpClient httpClient) : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<ApiEnvelope<T>> GetAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(path, cancellationToken);
        return await ReadEnvelopeAsync<T>(response, cancellationToken);
    }

    public async Task<ApiEnvelope<T>> PostAsync<T>(string path, object? body, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(path, body, JsonOptions, cancellationToken);
        return await ReadEnvelopeAsync<T>(response, cancellationToken);
    }

    public async Task<ApiEnvelope> PostAsync(string path, object? body, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(path, body, JsonOptions, cancellationToken);
        return await ReadEnvelopeAsync(response, cancellationToken);
    }

    public async Task<ApiEnvelope> PutAsync(string path, object? body, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(path, body, JsonOptions, cancellationToken);
        return await ReadEnvelopeAsync(response, cancellationToken);
    }

    public async Task<ApiEnvelope> DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync(path, cancellationToken);
        return await ReadEnvelopeAsync(response, cancellationToken);
    }

    public async Task<ApiEnvelope<T>> PostFileAsync<T>(string path, MultipartFormDataContent content, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(path, content, cancellationToken);
        return await ReadEnvelopeAsync<T>(response, cancellationToken);
    }

    private static async Task<ApiEnvelope<T>> ReadEnvelopeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        // A routing miss (wrong path, no matching endpoint) returns an empty body, not an enveloped
        // error - only the app's own ResponseResultHandler produces a parseable envelope.
        if (response.Content.Headers.ContentLength is 0 or null)
        {
            return new ApiEnvelope<T>(default, false, (int)response.StatusCode, response.ReasonPhrase, null);
        }

        var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<T>>(JsonOptions, cancellationToken);
        return envelope ?? new ApiEnvelope<T>(default, false, (int)response.StatusCode, response.ReasonPhrase, null);
    }

    private static async Task<ApiEnvelope> ReadEnvelopeAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentLength is 0 or null)
        {
            return new ApiEnvelope(false, (int)response.StatusCode, response.ReasonPhrase, null);
        }

        var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope>(JsonOptions, cancellationToken);
        return envelope ?? new ApiEnvelope(false, (int)response.StatusCode, response.ReasonPhrase, null);
    }
}
