using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Newsletter;

public class NewsletterService(IApiClient apiClient) : INewsletterService
{
    private const string NewsletterPath = "/api/inquiries/newsletter";

    /// <summary>The Api paginates GetSubscribers - the legacy admin screen has no pagination UI, so this requests
    /// a large single page to preserve the old "show everything" behavior without redesigning the view.</summary>
    public async Task<List<NewsletterListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<PaginatedSubscribers>($"{NewsletterPath}?index=0&size=1000", cancellationToken);
        var items = result.ResultData?.Items ?? [];
        return items.Select(s => new NewsletterListItemViewModel(s.Id, s.Email, s.CreatedDate)).ToList();
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{NewsletterPath}/{id}", cancellationToken);

    private record NewsletterSubscriberResponse(Guid Id, string Email, DateTimeOffset CreatedDate);
    private record PaginatedSubscribers(List<NewsletterSubscriberResponse> Items, int Index, int Size, int Count, int Pages);
}
