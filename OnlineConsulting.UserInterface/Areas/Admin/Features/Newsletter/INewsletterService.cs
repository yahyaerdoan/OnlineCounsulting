using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Newsletter;

/// <summary>All Api orchestration for the admin Newsletter subscribers screen.</summary>
public interface INewsletterService
{
    /// <summary>Fetches all newsletter subscribers.</summary>
    Task<List<NewsletterListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Deletes a subscriber by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
