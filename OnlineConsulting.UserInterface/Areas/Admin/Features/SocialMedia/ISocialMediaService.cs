using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SocialMedia;

/// <summary>All Api orchestration for the Social Media admin screens (site-wide header/footer social links,
/// backed by SiteContent's SocialLink entity) - SocialMediaController only calls this and renders the result.</summary>
public interface ISocialMediaService
{
    /// <summary>Fetches all social links.</summary>
    Task<List<SocialMediaListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single social link for editing, or null if not found.</summary>
    Task<UpdateSocialMediaViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new social link.</summary>
    Task<ApiEnvelope> CreateAsync(CreateSocialMediaViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing social link.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateSocialMediaViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a social link by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
