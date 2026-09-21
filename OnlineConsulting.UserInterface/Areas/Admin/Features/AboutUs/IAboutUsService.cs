using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.AboutUs;

public interface IAboutUsService
{
    /// <summary>Lists all About Us entries.</summary>
    Task<List<AboutUsListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets one About Us entry for editing, or null if not found.</summary>
    Task<UpdateAboutUsViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new About Us entry, uploading the cover image if provided.</summary>
    Task<ApiEnvelope> CreateAsync(CreateAboutUsViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an About Us entry, keeping the existing cover image when none is uploaded.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateAboutUsViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes an About Us entry.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
