using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Category;

/// <summary>View-model orchestration for the Category admin screens on top of the shared
/// Services.ICategoryService Api wrapper (named Admin* so it doesn't collide with that one).</summary>
public interface IAdminCategoryService
{
    Task<List<CategoryListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateCategoryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateCategoryViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateCategoryViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
