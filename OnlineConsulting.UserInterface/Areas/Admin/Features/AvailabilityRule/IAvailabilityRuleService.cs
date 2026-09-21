using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.AvailabilityRule;

/// <summary>The Api has no Update endpoint for this resource - only Create and Delete.</summary>
public interface IAvailabilityRuleService
{
    Task<List<AvailabilityRuleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateAvailabilityRuleViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
