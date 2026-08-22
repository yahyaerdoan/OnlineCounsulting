namespace OnlineConsulting.UserInterface.Features.Membership;

public record MembershipPlanCatalogItemViewModel(
    Guid Id,
    string Name,
    string BillingCycle,
    decimal Price,
    int IncludedVisitsPerYear,
    decimal DiscountPercent,
    string? Benefits);
