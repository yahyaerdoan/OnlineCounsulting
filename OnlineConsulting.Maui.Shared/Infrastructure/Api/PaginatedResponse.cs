namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Core.PersistenceLayer's generic Paginate envelope every paged Api endpoint returns.</summary>
public record PaginatedResponse<T>(List<T> Items, int Index, int Size, int Count, int Pages);
