using System.Text.Json.Serialization;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

/// <summary>A resource's "_links" by relation name; a missing relation means the caller can't take that action now.</summary>
[JsonConverter(typeof(HalLinksJsonConverter))]
public sealed class HalLinks(IReadOnlyDictionary<string, HalLink> links)
{
    public static readonly HalLinks Empty = new(new Dictionary<string, HalLink>());

    /// <summary>True when the Api offered this relation.</summary>
    public bool Has(string rel) => links.ContainsKey(rel);

    /// <summary>The link for the relation, or null when the Api didn't offer it.</summary>
    public HalLink? Find(string rel) => links.GetValueOrDefault(rel);
}
