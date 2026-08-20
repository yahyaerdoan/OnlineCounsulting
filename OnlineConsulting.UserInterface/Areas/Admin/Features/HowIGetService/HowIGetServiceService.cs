using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.HowIGetService;

public class HowIGetServiceService(IApiClient apiClient) : IHowIGetServiceService
{
    private const string ServiceProcessStepsPath = "/api/site-content/service-process-steps";

    public async Task<List<HowIGetServiceListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var steps = (await apiClient.GetAsync<List<ServiceProcessStepResponse>>(ServiceProcessStepsPath, cancellationToken)).ResultData ?? [];
        return steps.Select(s => new HowIGetServiceListItemViewModel(s.Id, s.Title, s.Description, s.Icon, s.IconColor)).ToList();
    }

    public async Task<UpdateHowIGetServiceViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var step = await FindAsync(id, cancellationToken);
        return step is null
            ? null
            : new UpdateHowIGetServiceViewModel
            {
                Id = step.Id,
                Title = step.Title,
                Description = step.Description,
                Icon = step.Icon,
                IconColor = step.IconColor,
            };
    }

    public Task<ApiEnvelope> CreateAsync(CreateHowIGetServiceViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(ServiceProcessStepsPath, new
        {
            model.Title,
            model.Description,
            model.Icon,
            model.IconColor,
        }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateHowIGetServiceViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{ServiceProcessStepsPath}/{model.Id}", new
        {
            model.Title,
            model.Description,
            model.Icon,
            model.IconColor,
        }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{ServiceProcessStepsPath}/{id}", cancellationToken);

    private async Task<ServiceProcessStepResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<List<ServiceProcessStepResponse>>(ServiceProcessStepsPath, cancellationToken);
        return result.ResultData?.FirstOrDefault(s => s.Id == id);
    }

    private record ServiceProcessStepResponse(Guid Id, string Title, string Description, string Icon, string? IconColor, int DisplayOrder);
}
