using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Promotion;

/// <summary>All Api orchestration for the Promotion admin screens - PromotionController only calls this and
/// renders the result, it never talks to IApiClient directly.</summary>
public interface IPromotionService
{
    /// <summary>Fetches all promotions.</summary>
    Task<List<PromotionListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single promotion for editing, or null if not found.</summary>
    Task<UpdatePromotionViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new promotion.</summary>
    Task<ApiEnvelope> CreateAsync(CreatePromotionViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing promotion.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdatePromotionViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a promotion by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
