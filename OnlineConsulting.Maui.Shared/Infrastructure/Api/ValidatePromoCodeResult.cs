namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/memberships/promo-codes/validate's response shape. IsValid=false is a normal
/// 200 with Error set - not an API error - so the caller renders Error inline instead of a toast.</summary>
public record ValidatePromoCodeResult(bool IsValid, string? Error, decimal DiscountAmount, decimal FinalPrice);
