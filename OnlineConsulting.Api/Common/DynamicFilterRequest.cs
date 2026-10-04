using Core.PersistenceLayer.Dynamics.Dynamic;

namespace OnlineConsulting.Api.Common;

/// <summary>One condition; nested <see cref="Filters"/> combine with it through <see cref="Logic"/> (and/or).</summary>
public record DynamicFilterRequest(string Field, string Operator, string? Value = null, string? Logic = null, IReadOnlyList<DynamicFilterRequest>? Filters = null)
{
    public Filter ToFilter() => new(Field, Operator) { Value = Value, Logic = Logic, Filters = Filters?.Select(f => f.ToFilter()).ToList() };
}
