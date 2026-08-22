using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.MembershipPlan;

public record MembershipPlanListItemViewModel(
    Guid Id,
    string Name,
    string BillingCycle,
    decimal Price,
    int IncludedVisitsPerYear,
    decimal DiscountPercent,
    decimal CreditAmount,
    string? Benefits);

public class CreateMembershipPlanViewModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string BillingCycle { get; set; } = "Monthly";

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int IncludedVisitsPerYear { get; set; }

    [Range(0, 100)]
    public decimal DiscountPercent { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CreditAmount { get; set; }

    [MaxLength(2000)]
    public string? Benefits { get; set; }
}

public record MembershipSubscriberListItemViewModel(
    Guid Id,
    string CustomerName,
    string PlanName,
    string Status,
    DateTimeOffset StartDate,
    DateTimeOffset? RenewalDate);

public class UpdateMembershipPlanViewModel
{
    public Guid Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Read-only reference fields - the Api's UpdateMembershipPlanCommand cannot change these
    /// (a real price change requires creating a new plan, per the Api's own doc comment).</summary>
    public string BillingCycle { get; set; } = string.Empty;

    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int IncludedVisitsPerYear { get; set; }

    [Range(0, 100)]
    public decimal DiscountPercent { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CreditAmount { get; set; }

    [MaxLength(2000)]
    public string? Benefits { get; set; }
}
