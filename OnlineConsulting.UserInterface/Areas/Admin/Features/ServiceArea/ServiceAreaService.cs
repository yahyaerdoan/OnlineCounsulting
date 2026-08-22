using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ServiceArea;

public class ServiceAreaService(IApiClient apiClient) : IServiceAreaService
{
    private const string ServiceAreasPath = "/api/site-content/service-areas";

    public async Task<List<ServiceAreaListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var serviceAreas = (await apiClient.GetAsync<List<ServiceAreaResponse>>(ServiceAreasPath, cancellationToken)).ResultData ?? [];
        return serviceAreas.Select(s => new ServiceAreaListItemViewModel(s.Id, s.Name, s.State, s.Slug, s.IntroText, s.DisplayOrder)).ToList();
    }

    public async Task<UpdateServiceAreaViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var serviceArea = await FindAsync(id, cancellationToken);
        return serviceArea is null
            ? null
            : new UpdateServiceAreaViewModel
            {
                Id = serviceArea.Id,
                Name = serviceArea.Name,
                State = serviceArea.State,
                Slug = serviceArea.Slug,
                IntroText = serviceArea.IntroText,
                DisplayOrder = serviceArea.DisplayOrder,
            };
    }

    public Task<ApiEnvelope> CreateAsync(CreateServiceAreaViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(ServiceAreasPath, new
        {
            model.Name,
            model.State,
            model.IntroText,
            model.DisplayOrder,
        }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateServiceAreaViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{ServiceAreasPath}/{model.Id}", new
        {
            model.Name,
            model.State,
            model.IntroText,
            model.DisplayOrder,
        }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{ServiceAreasPath}/{id}", cancellationToken);

    private async Task<ServiceAreaResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<ServiceAreaResponse>>(ServiceAreasPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(s => s.Id == id);
    }

    private record ServiceAreaResponse(Guid Id, string Name, string State, string Slug, string? IntroText, int DisplayOrder);
}
