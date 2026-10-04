namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/membership-plans's response shape.</summary>
public record MembershipPlanResponse(Guid Id, string Name, string BillingCycle, decimal Price, int IncludedVisitsPerYear, decimal DiscountPercent, decimal CreditAmount, string? Benefits, bool IsActive, int? TrialDays = null);
