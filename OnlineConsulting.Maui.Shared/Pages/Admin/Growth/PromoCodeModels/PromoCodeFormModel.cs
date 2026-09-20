namespace OnlineConsulting.Maui.Shared.Pages.Admin.Growth.PromoCodeModels;

public class PromoCodeFormModel
{
    public string Code { get; set; } = string.Empty;

    public string DiscountType { get; set; } = "Percent";

    public decimal DiscountValue { get; set; }

    public int? MaxRedemptions { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public Guid? MembershipPlanId { get; set; }
}
