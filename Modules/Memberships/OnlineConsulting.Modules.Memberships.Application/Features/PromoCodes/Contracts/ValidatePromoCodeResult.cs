namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Contracts;

public record ValidatePromoCodeResult(bool IsValid, string? Error, decimal DiscountAmount, decimal FinalPrice);
