namespace OnlineConsulting.Maui.Shared.Pages.Admin.Growth.MembershipPlanModels;

public class MembershipPlanFormModel
{
    public string Name { get; set; } = string.Empty;

    public string BillingCycle { get; set; } = "Monthly";

    public decimal Price { get; set; }

    public int IncludedVisitsPerYear { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal CreditAmount { get; set; }

    public string? Benefits { get; set; }
}
