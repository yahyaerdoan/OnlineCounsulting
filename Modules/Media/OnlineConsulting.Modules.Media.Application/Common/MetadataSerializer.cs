using System.Text.Json;

namespace OnlineConsulting.Modules.Media.Application.Common;

/// <summary>Metadata is stored as a plain JSON string column - this is the single point where that string is produced/parsed, so an invalid payload can never reach the database.</summary>
public static class MetadataSerializer
{
    public static string? Serialize(Dictionary<string, object>? metadata) =>
        metadata is null or { Count: 0 } ? null : JsonSerializer.Serialize(metadata);

    public static Dictionary<string, object>? Deserialize(string? metadata) =>
        metadata is null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(metadata);
}
