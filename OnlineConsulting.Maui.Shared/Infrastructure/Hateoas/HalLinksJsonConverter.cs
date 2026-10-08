using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

/// <summary>Reads HAL "_links": single links become <see cref="HalLink"/>s, arrays (CURIEs) are skipped, null or absent is empty.</summary>
public sealed class HalLinksJsonConverter : JsonConverter<HalLinks>
{
    public override bool HandleNull => true;

    public override HalLinks Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return HalLinks.Empty;
        }

        using var document = JsonDocument.ParseValue(ref reader);
        var links = new Dictionary<string, HalLink>(StringComparer.Ordinal);
        foreach (var property in document.RootElement.EnumerateObject().Where(property => property.Value.ValueKind == JsonValueKind.Object))
        {
            if (property.Value.TryGetProperty("href", out var href) && href.GetString() is { } value)
            {
                links[property.Name] = new HalLink(value, property.Value.TryGetProperty("method", out var method) ? method.GetString() : null);
            }
        }

        return new HalLinks(links);
    }

    public override void Write(Utf8JsonWriter writer, HalLinks value, JsonSerializerOptions options) =>
        throw new NotSupportedException("HAL links are only read from the Api.");
}
