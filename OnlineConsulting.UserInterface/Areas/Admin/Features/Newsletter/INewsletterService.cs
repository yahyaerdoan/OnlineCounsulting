using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Newsletter;

/// <summary>All Api orchestration for the admin Newsletter subscribers screen.</summary>
public interface INewsletterService
{
    Task<List<NewsletterListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
