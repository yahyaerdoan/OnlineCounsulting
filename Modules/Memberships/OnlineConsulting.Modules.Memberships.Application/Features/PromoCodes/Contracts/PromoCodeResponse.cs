using Hateoas;
using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Contracts;

/// <summary>A class with required init properties instead of a positional record, since records can't inherit LinkedResponse.</summary>
public class PromoCodeResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string DiscountType { get; init; }
    public required decimal DiscountValue { get; init; }
    public int? MaxRedemptions { get; init; }
    public required int RedemptionCount { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public required bool IsActive { get; init; }
    public Guid? MembershipPlanId { get; init; }

    public static PromoCodeResponse FromDomain(PromoCode promoCode) => new()
    {
        Id = promoCode.Id,
        Code = promoCode.Code,
        DiscountType = promoCode.DiscountType,
        DiscountValue = promoCode.DiscountValue,
        MaxRedemptions = promoCode.MaxRedemptions,
        RedemptionCount = promoCode.RedemptionCount,
        ExpiresAt = promoCode.ExpiresAt,
        IsActive = promoCode.IsActive,
        MembershipPlanId = promoCode.MembershipPlanId,
    };
}
