namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Core.PersistenceLayer's DynamicQuery (Sort + Filter), sent as a POST body.</summary>
public record DynamicQuery(List<DynamicSort>? Sort = null, DynamicFilter? Filter = null)
{
    /// <summary>Matches one row by Id, for edit forms whose resource has no single-item GET; pair with a page size of 1.</summary>
    public static DynamicQuery ById(Guid id) => new(null, new DynamicFilter("Id", "eq", id.ToString()));
}

public record DynamicSort(string Field, string Direction);

public record DynamicFilter(string Field, string Operator, string? Value = null, string? Logic = null, List<DynamicFilter>? Filters = null);
