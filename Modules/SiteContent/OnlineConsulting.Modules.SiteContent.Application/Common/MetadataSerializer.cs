using System.Text.Json;

namespace OnlineConsulting.Modules.SiteContent.Application.Common;

/// <summary>Metadata is stored as a plain JSON string column (no schema enforced at the DB level, so a different UI template can stash whatever extra fields it needs without a migration) - this is the single point where that string is produced/parsed, so an invalid payload can never reach the database.</summary>
public static class MetadataSerializer
{
    public static string? Serialize(Dictionary<string, object>? metadata) =>
        metadata is null or { Count: 0 } ? null : JsonSerializer.Serialize(metadata);

    public static Dictionary<string, object>? Deserialize(string? metadata) =>
        metadata is null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(metadata);
}
