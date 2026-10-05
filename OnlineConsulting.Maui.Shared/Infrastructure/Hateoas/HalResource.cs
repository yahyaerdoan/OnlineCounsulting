using System.Text.Json.Serialization;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

/// <summary>Base for Api resources that carry "_links": the Api decides which actions the caller may take, the UI only asks.</summary>
public abstract record HalResource
{
    [JsonPropertyName("_links")]
    public HalLinks Links { get; init; } = HalLinks.Empty;

    /// <summary>True when the Api offered the action (state and permissions both allow it).</summary>
    public bool Can(string rel) => Links.Has(rel);
}
