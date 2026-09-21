namespace OnlineConsulting.UserInterface.Features.Search;

public interface ISearchService
{
    /// <summary>Searches services by title/description; returns an empty list for a blank query.</summary>
    Task<List<SearchResultItemViewModel>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
