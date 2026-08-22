using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Bundle;

public class BundleService(IApiClient apiClient) : IBundleService
{
    private const string BundlesPath = "/api/tenancy/admin/bundles";
    private const string ModuleOfferingsPath = "/api/tenancy/admin/module-offerings";

    public async Task<List<BundleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<Paginated<BundleResponse>>($"{BundlesPath}?size=100", cancellationToken);
        var bundles = result.ResultData?.Items ?? [];

        return [.. bundles.Select(b => new BundleListItemViewModel(b.Id, b.Name, b.ModuleKeys, b.IsPubliclyVisible))];
    }

    public async Task<CreateBundleViewModel> GetCreateFormAsync(CancellationToken cancellationToken = default) =>
        new() { AvailableModuleKeys = await FetchAvailableModuleKeysAsync(cancellationToken) };

    public async Task<UpdateBundleViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bundleTask = apiClient.GetAsync<BundleResponse>($"{BundlesPath}/{id}", cancellationToken);
        var keysTask = FetchAvailableModuleKeysAsync(cancellationToken);
        await Task.WhenAll(bundleTask, keysTask);

        var bundle = bundleTask.Result.ResultData;
        return bundle is null
            ? null
            : new UpdateBundleViewModel
            {
                Id = bundle.Id,
                Name = bundle.Name,
                ModuleKeys = bundle.ModuleKeys,
                IsPubliclyVisible = bundle.IsPubliclyVisible,
                AvailableModuleKeys = keysTask.Result,
            };
    }

    public Task<ApiEnvelope> CreateAsync(CreateBundleViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(BundlesPath, new
        {
            model.Name,
            model.ModuleKeys,
            model.IsPubliclyVisible,
        }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateBundleViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{BundlesPath}/{model.Id}", new
        {
            model.Name,
            model.ModuleKeys,
            model.IsPubliclyVisible,
        }, cancellationToken);

    private async Task<List<string>> FetchAvailableModuleKeysAsync(CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<Paginated<ModuleOfferingResponse>>($"{ModuleOfferingsPath}?size=100", cancellationToken);
        return [.. (result.ResultData?.Items ?? []).Select(m => m.Key)];
    }

    private record BundleResponse(Guid Id, string Name, List<string> ModuleKeys, bool IsPubliclyVisible);
    private record ModuleOfferingResponse(Guid Id, string Key, string Name, decimal Price, string BillingCycle, bool IsPubliclyVisible);
    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
