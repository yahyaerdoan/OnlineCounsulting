namespace OnlineConsulting.UserInterface.Features.Search;

public interface ISearchService
{
    Task<List<SearchResultItemViewModel>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
