using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Tenant;

public class TenantService(IApiClient apiClient) : ITenantService
{
    private const string TenantsPath = "/api/tenancy/admin/tenants";
    private const string ModuleOfferingsPath = "/api/tenancy/module-offerings";

    public async Task<List<TenantListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<Paginated<TenantSummaryResponse>>($"{TenantsPath}?size=100", cancellationToken);
        var tenants = result.ResultData?.Items ?? [];

        return [.. tenants.Select(t => new TenantListItemViewModel(
            t.Id, t.Name, t.Slug, t.Status, t.PrimaryContactEmail, t.ActiveModuleKeys, t.TotalActivePrice))];
    }

    public async Task<TenantDetailViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var detailTask = apiClient.GetAsync<TenantDetailResponse>($"{TenantsPath}/{id}", cancellationToken);
        var offeringsTask = apiClient.GetAsync<List<ModuleOfferingResponse>>(ModuleOfferingsPath, cancellationToken);
        await Task.WhenAll(detailTask, offeringsTask);

        var tenant = detailTask.Result.ResultData;
        if (tenant is null)
        {
            return null;
        }

        var activeModuleKeys = tenant.Items.Where(i => i.Status == "Active").Select(i => i.ModuleKey).ToHashSet();
        var availableModules = (offeringsTask.Result.ResultData ?? [])
            .Where(m => !activeModuleKeys.Contains(m.Key))
            .Select(m => new ModuleOfferingViewModel(m.Key, m.Name, m.Price, m.BillingCycle))
            .ToList();

        return new TenantDetailViewModel(
            tenant.Id, tenant.Name, tenant.Slug, tenant.Status, tenant.PrimaryContactEmail, tenant.OwnerUserId,
            tenant.SubscriptionStatus, tenant.SubscriptionStartDate, tenant.SubscriptionRenewalDate,
            [.. tenant.Items.Select(i => new TenantSubscriptionItemViewModel(i.ModuleKey, i.Status, i.PriceAtAddition, i.AddedAt))],
            availableModules);
    }

    public Task<ApiEnvelope> SuspendAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{TenantsPath}/{id}/suspend", null, cancellationToken);

    public Task<ApiEnvelope> ReactivateAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{TenantsPath}/{id}/reactivate", null, cancellationToken);

    public Task<ApiEnvelope> AddModuleAsync(Guid id, string moduleKey, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"/api/tenancy/{id}/modules/{moduleKey}", null, cancellationToken);

    public Task<ApiEnvelope> RemoveModuleAsync(Guid id, string moduleKey, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"/api/tenancy/{id}/modules/{moduleKey}", cancellationToken);

    private record TenantSummaryResponse(Guid Id, string Name, string Slug, string Status, string PrimaryContactEmail, List<string> ActiveModuleKeys, decimal TotalActivePrice);
    private record TenantSubscriptionItemSummaryResponse(string ModuleKey, string Status, decimal PriceAtAddition, DateTime AddedAt);
    private record TenantDetailResponse(
        Guid Id, string Name, string Slug, string Status, string PrimaryContactEmail, Guid? OwnerUserId,
        string? SubscriptionStatus, DateTime? SubscriptionStartDate, DateTime? SubscriptionRenewalDate,
        List<TenantSubscriptionItemSummaryResponse> Items);
    private record ModuleOfferingResponse(string Key, string Name, decimal Price, string BillingCycle);
    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
