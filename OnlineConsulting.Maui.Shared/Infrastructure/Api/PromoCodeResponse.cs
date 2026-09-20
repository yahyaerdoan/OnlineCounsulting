namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/promo-codes's response shape.</summary>
public record PromoCodeResponse(Guid Id, string Code, string DiscountType, decimal DiscountValue, int? MaxRedemptions, int RedemptionCount, DateTimeOffset? ExpiresAt, bool IsActive, Guid? MembershipPlanId);
