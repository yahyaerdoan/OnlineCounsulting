using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Membership;

public class SubscribeMembershipViewModel
{
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string BillingCycle { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CreditBalance { get; set; }

    [Required]
    public string PaymentMethodId { get; set; } = string.Empty;

    public bool ApplyCredit { get; set; }
}

public record MyMembershipViewModel(
    bool HasMembership,
    string? PlanName,
    string? Status,
    DateTimeOffset? StartDate,
    DateTimeOffset? RenewalDate,
    bool CanCancel);
