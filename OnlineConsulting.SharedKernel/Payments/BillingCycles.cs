namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>MembershipPlan.BillingCycle / EnsurePriceRequest.BillingCycle vocabulary - shared between the Memberships module and the Payments project (which maps these to the provider's own recurring-interval vocabulary, e.g. Stripe's "month"/"year") so neither has to reference the other's assembly.</summary>
public static class BillingCycles
{
    public const string Monthly = "Monthly";
    public const string Annual = "Annual";
}
