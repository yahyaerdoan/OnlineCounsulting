using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Addresses;

/// <summary>Geoapify autocomplete limited to US street addresses. Answers are cached for a day (free tier is 3000 requests a day), and any
/// provider failure returns no suggestions so typing the address by hand always works.</summary>
public sealed class GeoapifyAddressSuggestionProvider(IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<GeoapifyOptions> options,
    ILogger<GeoapifyAddressSuggestionProvider> logger) : IAddressSuggestionProvider
{
    public const string HttpClientName = "Geoapify";

    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<IReadOnlyList<AddressSuggestionResponse>> SuggestAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(options.Value.ApiKey))
        {
            return [];
        }

        var cacheKey = $"geoapify:{text.ToLowerInvariant()}";
        if (cache.TryGetValue(cacheKey, out IReadOnlyList<AddressSuggestionResponse>? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            var client = httpClientFactory.CreateClient(HttpClientName);
            var url = $"v1/geocode/autocomplete?text={Uri.EscapeDataString(text)}&filter=countrycode:us&type=street&format=json&limit=6&lang=en&apiKey={Uri.EscapeDataString(options.Value.ApiKey)}";
            var response = await client.GetFromJsonAsync<GeoapifyResponse>(url, cancellationToken);

            IReadOnlyList<AddressSuggestionResponse> suggestions = [.. (response?.Results ?? [])
                .Where(r => !string.IsNullOrWhiteSpace(r.AddressLine1))
                .Select(r => new AddressSuggestionResponse(
                    r.Formatted ?? r.AddressLine1 ?? "",
                    r.AddressLine1 ?? "",
                    r.City ?? r.Town ?? r.Village ?? "",
                    r.StateCode?.ToUpperInvariant() ?? r.State ?? "",
                    r.Postcode ?? ""))
                .DistinctBy(s => s.Formatted)];

            _ = cache.Set(cacheKey, suggestions, CacheDuration);
            return suggestions;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Geoapify address autocomplete failed.");
            return [];
        }
    }

    private sealed record GeoapifyResponse([property: JsonPropertyName("results")] List<GeoapifyResult>? Results);

    private sealed record GeoapifyResult(
        [property: JsonPropertyName("formatted")] string? Formatted,
        [property: JsonPropertyName("address_line1")] string? AddressLine1,
        [property: JsonPropertyName("city")] string? City,
        [property: JsonPropertyName("town")] string? Town,
        [property: JsonPropertyName("village")] string? Village,
        [property: JsonPropertyName("state")] string? State,
        [property: JsonPropertyName("state_code")] string? StateCode,
        [property: JsonPropertyName("postcode")] string? Postcode);
}
