using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Geocoding;

/// <summary>Geoapify when a key is configured (Geoapify:ApiKey, shared with address autocomplete), otherwise OpenStreetMap Nominatim, which
/// allows light use with an identifying User-Agent and at most one request a second. Results are cached; failures return null.</summary>
public sealed class CityGeocoder(IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<GeoapifyOptions> options, ILogger<CityGeocoder> logger) : ICityGeocoder
{
    public const string GeoapifyClient = "SiteContent.Geoapify";
    public const string NominatimClient = "SiteContent.Nominatim";

    public async Task<GeoPoint?> GeocodeAsync(string city, string state, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return null;
        }

        var key = $"city-geo:{city.Trim().ToLowerInvariant()}|{state.Trim().ToLowerInvariant()}";
        if (cache.TryGetValue(key, out GeoPoint cached))
        {
            return cached;
        }

        try
        {
            var apiKey = options.Value.ApiKey;
            var point = string.IsNullOrWhiteSpace(apiKey)
                ? await FromNominatimAsync(city, state, cancellationToken)
                : await FromGeoapifyAsync(city, state, apiKey, cancellationToken);

            if (point is { } found)
            {
                _ = cache.Set(key, found, TimeSpan.FromDays(30));
            }

            return point;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Geocoding {City}, {State} failed.", city, state);
            return null;
        }
    }

    private async Task<GeoPoint?> FromGeoapifyAsync(string city, string state, string apiKey, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(GeoapifyClient);
        var url = $"v1/geocode/search?city={Uri.EscapeDataString(city)}&state={Uri.EscapeDataString(state)}&filter=countrycode:us&type=city&limit=1&format=json&apiKey={Uri.EscapeDataString(apiKey)}";
        var response = await client.GetFromJsonAsync<GeoapifyResponse>(url, cancellationToken);
        return response?.Results is [var first, ..] ? new GeoPoint(first.Lat, first.Lon) : null;
    }

    private async Task<GeoPoint?> FromNominatimAsync(string city, string state, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(NominatimClient);
        var url = $"search?city={Uri.EscapeDataString(city)}&state={Uri.EscapeDataString(state)}&country=USA&format=json&limit=1";
        var results = await client.GetFromJsonAsync<List<NominatimResult>>(url, cancellationToken);
        return results is [var first, ..]
            && double.TryParse(first.Lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat)
            && double.TryParse(first.Lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon)
            ? new GeoPoint(lat, lon)
            : null;
    }

    private sealed record GeoapifyResponse([property: JsonPropertyName("results")] List<GeoapifyResult>? Results);

    private sealed record GeoapifyResult([property: JsonPropertyName("lat")] double Lat, [property: JsonPropertyName("lon")] double Lon);

    private sealed record NominatimResult([property: JsonPropertyName("lat")] string Lat, [property: JsonPropertyName("lon")] string Lon);
}
