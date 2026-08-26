namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Lets ServerDataTable derive its search box fields from TItem itself - no per-page param.</summary>
public interface IQueryableFields
{
    static abstract string[] SearchFields { get; }
}
