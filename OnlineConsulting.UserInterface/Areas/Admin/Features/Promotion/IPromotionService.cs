using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Promotion;

/// <summary>All Api orchestration for the Promotion admin screens - PromotionController only calls this and
/// renders the result, it never talks to IApiClient directly.</summary>
public interface IPromotionService
{
    Task<List<PromotionListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdatePromotionViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreatePromotionViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdatePromotionViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
