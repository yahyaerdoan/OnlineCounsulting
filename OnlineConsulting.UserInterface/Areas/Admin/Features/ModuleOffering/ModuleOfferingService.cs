using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ModuleOffering;

public class ModuleOfferingService(IApiClient apiClient) : IModuleOfferingService
{
    private const string OfferingsPath = "/api/tenancy/admin/module-offerings";

    public async Task<List<ModuleOfferingListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<Paginated<ModuleOfferingResponse>>($"{OfferingsPath}?size=100", cancellationToken);
        var offerings = result.ResultData?.Items ?? [];

        return [.. offerings.Select(o => new ModuleOfferingListItemViewModel(o.Id, o.Key, o.Name, o.Price, o.BillingCycle, o.IsPubliclyVisible))];
    }

    public async Task<UpdateModuleOfferingViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<ModuleOfferingResponse>($"{OfferingsPath}/{id}", cancellationToken);
        var offering = result.ResultData;
        return offering is null
            ? null
            : new UpdateModuleOfferingViewModel
            {
                Id = offering.Id,
                Key = offering.Key,
                Name = offering.Name,
                Price = offering.Price,
                BillingCycle = offering.BillingCycle,
                IsPubliclyVisible = offering.IsPubliclyVisible,
            };
    }

    public Task<ApiEnvelope> CreateAsync(CreateModuleOfferingViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(OfferingsPath, new
        {
            model.Key,
            model.Name,
            model.Price,
            model.BillingCycle,
            model.IsPubliclyVisible,
        }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateModuleOfferingViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{OfferingsPath}/{model.Id}", new
        {
            model.Name,
            model.IsPubliclyVisible,
        }, cancellationToken);

    private record ModuleOfferingResponse(Guid Id, string Key, string Name, decimal Price, string BillingCycle, bool IsPubliclyVisible, string? ProviderProductId, string? ProviderPriceId);
    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
