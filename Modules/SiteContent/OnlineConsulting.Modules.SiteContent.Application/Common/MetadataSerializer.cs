using System.Text.Json;

namespace OnlineConsulting.Modules.SiteContent.Application.Common;

/// <summary>Single point where the schemaless Metadata JSON column is produced/parsed, so an invalid payload can never reach the database.</summary>
public static class MetadataSerializer
{
    public static string? Serialize(Dictionary<string, object>? metadata) =>
        metadata is null or { Count: 0 } ? null : JsonSerializer.Serialize(metadata);

    public static Dictionary<string, object>? Deserialize(string? metadata) =>
        metadata is null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(metadata);
}
