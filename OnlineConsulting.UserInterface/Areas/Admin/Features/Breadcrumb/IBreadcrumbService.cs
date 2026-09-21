using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Breadcrumb;

/// <summary>Api concept is called PageBanner - the C# type/route name stays Breadcrumb for continuity with the existing view folder.</summary>
public interface IBreadcrumbService
{
    /// <summary>Lists all page banners.</summary>
    Task<List<BreadcrumbListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets one page banner for editing, or null if not found.</summary>
    Task<UpdateBreadcrumbViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new page banner, uploading the image if provided.</summary>
    Task<ApiEnvelope> CreateAsync(CreateBreadcrumbViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates a page banner, keeping the existing image when none is uploaded.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateBreadcrumbViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a page banner.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
