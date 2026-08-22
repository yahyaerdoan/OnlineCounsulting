namespace OnlineConsulting.UserInterface.Features.Membership;

/// <summary>Public plan browsing - GET /api/membership-plans requires no auth ("used by the pricing page",
/// per the Api's own endpoint description).</summary>
public interface IMembershipPlanCatalogService
{
    Task<List<MembershipPlanCatalogItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
}
