namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/membership-plans's response shape.</summary>
public record MembershipPlanResponse(Guid Id, string Name, string BillingCycle, decimal Price, int IncludedVisitsPerYear, decimal DiscountPercent, decimal CreditAmount, string? Benefits);
