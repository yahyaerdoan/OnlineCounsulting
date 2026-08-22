using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.FeatureFlag;

public class FeatureFlagService(IApiClient apiClient) : IFeatureFlagService
{
    private const string FeatureFlagsPath = "/api/admin/feature-flags";

    public async Task<List<FeatureFlagListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var flags = (await apiClient.GetAsync<List<FeatureFlagResponse>>(FeatureFlagsPath, cancellationToken)).ResultData ?? [];
        return flags.Select(f => new FeatureFlagListItemViewModel(f.Key, f.IsEnabled, f.Price, f.IsPurchased)).ToList();
    }

    public Task<ApiEnvelope> ToggleAsync(string key, bool isEnabled, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{FeatureFlagsPath}/{key}", new { IsEnabled = isEnabled }, cancellationToken);

    private record FeatureFlagResponse(string Key, bool IsEnabled, decimal? Price, bool IsPurchased);
}
