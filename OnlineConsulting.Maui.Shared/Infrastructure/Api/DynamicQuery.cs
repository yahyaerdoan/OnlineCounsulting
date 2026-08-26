namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Core.PersistenceLayer's DynamicQuery (Sort + Filter), sent as a POST body.</summary>
public record DynamicQuery(List<DynamicSort>? Sort = null, DynamicFilter? Filter = null);

public record DynamicSort(string Field, string Direction);

public record DynamicFilter(string Field, string Operator, string? Value = null, string? Logic = null, List<DynamicFilter>? Filters = null);
