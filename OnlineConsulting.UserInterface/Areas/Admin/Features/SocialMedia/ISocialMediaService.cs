using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SocialMedia;

/// <summary>All Api orchestration for the Social Media admin screens (site-wide header/footer social links,
/// backed by SiteContent's SocialLink entity) - SocialMediaController only calls this and renders the result.</summary>
public interface ISocialMediaService
{
    Task<List<SocialMediaListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateSocialMediaViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateSocialMediaViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateSocialMediaViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
