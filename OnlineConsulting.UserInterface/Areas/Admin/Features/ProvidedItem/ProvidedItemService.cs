using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ProvidedItem;

public class ProvidedItemService(IApiClient apiClient) : IProvidedItemService
{
    private const string ServiceOfferingsPath = "/api/site-content/service-offerings";

    public async Task<List<ProvidedItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var offerings = (await apiClient.GetAsync<List<ServiceOfferingResponse>>(ServiceOfferingsPath, cancellationToken)).ResultData ?? [];
        return offerings.Select(o => new ProvidedItemListItemViewModel(o.Id, o.Title, o.Description, o.Icon, o.IconColor)).ToList();
    }

    public async Task<UpdateProvidedItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var offering = await FindAsync(id, cancellationToken);
        if (offering is null)
            return null;

        return new UpdateProvidedItemViewModel
        {
            Id = offering.Id,
            Title = offering.Title,
            Description = offering.Description,
            Icon = offering.Icon,
            IconColor = offering.IconColor,
        };
    }

    public Task<ApiEnvelope> CreateAsync(CreateProvidedItemViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(ServiceOfferingsPath, new
        {
            model.Title,
            model.Description,
            model.Icon,
            model.IconColor,
        }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateProvidedItemViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{ServiceOfferingsPath}/{model.Id}", new
        {
            model.Title,
            model.Description,
            model.Icon,
            model.IconColor,
        }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{ServiceOfferingsPath}/{id}", cancellationToken);

    private async Task<ServiceOfferingResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<ServiceOfferingResponse>>(ServiceOfferingsPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(o => o.Id == id);
    }

    private record ServiceOfferingResponse(Guid Id, string Title, string Description, string Icon, string? IconColor, int DisplayOrder);
}
