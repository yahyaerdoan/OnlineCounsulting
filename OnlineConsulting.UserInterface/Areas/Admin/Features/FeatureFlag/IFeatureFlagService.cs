using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.FeatureFlag;

/// <summary>All Api orchestration for the FeatureFlag admin screen - FeatureFlagController only calls this and
/// renders the result, it never talks to IApiClient directly.</summary>
public interface IFeatureFlagService
{
    Task<List<FeatureFlagListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> ToggleAsync(string key, bool isEnabled, CancellationToken cancellationToken = default);
}
