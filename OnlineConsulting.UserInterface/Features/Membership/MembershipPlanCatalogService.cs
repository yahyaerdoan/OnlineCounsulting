using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Membership;

public class MembershipPlanCatalogService(IApiClient apiClient) : IMembershipPlanCatalogService
{
    private const string PlansPath = "/api/membership-plans";

    public async Task<List<MembershipPlanCatalogItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<Paginated<MembershipPlanResponse>>($"{PlansPath}?size=100", cancellationToken);
        var plans = result.ResultData?.Items ?? [];
        return plans
            .OrderBy(p => p.Price)
            .Select(p => new MembershipPlanCatalogItemViewModel(p.Id, p.Name, p.BillingCycle, p.Price, p.IncludedVisitsPerYear, p.DiscountPercent, p.Benefits))
            .ToList();
    }

    private record MembershipPlanResponse(Guid Id, string Name, string BillingCycle, decimal Price, int IncludedVisitsPerYear, decimal DiscountPercent, string? Benefits);
    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
