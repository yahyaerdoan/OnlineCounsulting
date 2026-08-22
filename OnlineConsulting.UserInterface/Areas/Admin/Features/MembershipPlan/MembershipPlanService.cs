using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.MembershipPlan;

public class MembershipPlanService(IApiClient apiClient) : IMembershipPlanService
{
    private const string PlansPath = "/api/membership-plans";
    private const string MembershipsAdminPath = "/api/memberships";
    private const string UsersPath = "/api/users";

    public async Task<List<MembershipPlanListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var plans = await FetchPlansAsync(cancellationToken);
        return plans.Select(p => new MembershipPlanListItemViewModel(
            p.Id, p.Name, p.BillingCycle, p.Price, p.IncludedVisitsPerYear, p.DiscountPercent, p.CreditAmount, p.Benefits)).ToList();
    }

    public async Task<UpdateMembershipPlanViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<MembershipPlanResponse>($"{PlansPath}/{id}", cancellationToken);
        var plan = result.ResultData;
        return plan is null
            ? null
            : new UpdateMembershipPlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                BillingCycle = plan.BillingCycle,
                Price = plan.Price,
                IncludedVisitsPerYear = plan.IncludedVisitsPerYear,
                DiscountPercent = plan.DiscountPercent,
                CreditAmount = plan.CreditAmount,
                Benefits = plan.Benefits,
            };
    }

    public Task<ApiEnvelope> CreateAsync(CreateMembershipPlanViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(PlansPath, new
        {
            model.Name,
            model.BillingCycle,
            model.Price,
            model.IncludedVisitsPerYear,
            model.DiscountPercent,
            model.CreditAmount,
            model.Benefits,
        }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateMembershipPlanViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{PlansPath}/{model.Id}", new
        {
            model.Name,
            model.IncludedVisitsPerYear,
            model.DiscountPercent,
            model.CreditAmount,
            model.Benefits,
        }, cancellationToken);

    public async Task<List<MembershipSubscriberListItemViewModel>> GetSubscribersAsync(CancellationToken cancellationToken = default)
    {
        var membershipsTask = apiClient.GetAsync<Paginated<CustomerMembershipResponse>>($"{MembershipsAdminPath}?size=100", cancellationToken);
        var usersTask = apiClient.GetAsync<List<UserResponse>>(UsersPath, cancellationToken);
        var plansTask = FetchPlansAsync(cancellationToken);
        await Task.WhenAll(membershipsTask, usersTask, plansTask);

        var usersById = (usersTask.Result.ResultData ?? []).ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim());
        var plansById = plansTask.Result.ToDictionary(p => p.Id, p => p.Name);
        var memberships = membershipsTask.Result.ResultData?.Items ?? [];

        return memberships
            .OrderByDescending(m => m.StartDate)
            .Select(m => new MembershipSubscriberListItemViewModel(
                m.Id,
                usersById.TryGetValue(m.UserId, out var name) ? name : "Unknown customer",
                plansById.TryGetValue(m.MembershipPlanId, out var planName) ? planName : "Unknown plan",
                m.Status,
                m.StartDate,
                m.RenewalDate))
            .ToList();
    }

    private async Task<List<MembershipPlanResponse>> FetchPlansAsync(CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<Paginated<MembershipPlanResponse>>($"{PlansPath}?size=100", cancellationToken);
        return result.ResultData?.Items ?? [];
    }

    private record MembershipPlanResponse(Guid Id, string Name, string BillingCycle, decimal Price, int IncludedVisitsPerYear, decimal DiscountPercent, decimal CreditAmount, string? Benefits);
    private record CustomerMembershipResponse(Guid Id, Guid UserId, Guid MembershipPlanId, string Status, DateTimeOffset StartDate, DateTimeOffset? RenewalDate);
    private record UserResponse(Guid Id, string FirstName, string LastName);
    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
