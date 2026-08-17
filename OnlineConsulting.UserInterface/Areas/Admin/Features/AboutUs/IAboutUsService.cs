using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.AboutUs;

/// <summary>All Api orchestration for the AboutUs admin screens.</summary>
public interface IAboutUsService
{
    Task<List<AboutUsListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateAboutUsViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateAboutUsViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateAboutUsViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
