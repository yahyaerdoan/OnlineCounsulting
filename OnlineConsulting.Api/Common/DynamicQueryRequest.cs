using Core.PersistenceLayer.Dynamics.Dynamic;

namespace OnlineConsulting.Api.Common;

/// <summary>Body of every POST .../query list endpoint: optional sort and filter, kept separate from the persistence DynamicQuery so the wire format is the API's own.</summary>
public record DynamicQueryRequest(IReadOnlyList<DynamicSortRequest>? Sort = null, DynamicFilterRequest? Filter = null)
{
    public DynamicQuery ToDynamicQuery() => new([.. (Sort ?? []).Select(s => s.ToSort())], Filter?.ToFilter());
}
