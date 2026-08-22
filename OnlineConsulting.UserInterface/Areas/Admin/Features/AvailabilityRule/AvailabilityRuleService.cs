using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.AvailabilityRule;

public class AvailabilityRuleService(IApiClient apiClient) : IAvailabilityRuleService
{
    private const string AvailabilityRulesPath = "/api/scheduling/availability-rules";

    public async Task<List<AvailabilityRuleListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rules = (await apiClient.GetAsync<List<AvailabilityRuleResponse>>(AvailabilityRulesPath, cancellationToken)).ResultData ?? [];
        return rules
            .OrderBy(r => r.DayOfWeek)
            .ThenBy(r => r.StartTime)
            .Select(r => new AvailabilityRuleListItemViewModel(r.Id, r.DayOfWeek, r.StartTime, r.EndTime, r.SlotDurationMinutes))
            .ToList();
    }

    public Task<ApiEnvelope> CreateAsync(CreateAvailabilityRuleViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(AvailabilityRulesPath, new
        {
            model.DayOfWeek,
            StartTime = TimeSpan.Parse(model.StartTime),
            EndTime = TimeSpan.Parse(model.EndTime),
            model.SlotDurationMinutes,
        }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{AvailabilityRulesPath}/{id}", cancellationToken);

    private record AvailabilityRuleResponse(Guid Id, DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int SlotDurationMinutes);
}
