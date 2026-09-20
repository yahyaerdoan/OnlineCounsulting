namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Shared BillingCycle vocabulary between Memberships and Payments; Payments maps these to the provider's own interval strings (e.g. Stripe's "month"/"year").</summary>
public static class BillingCycles
{
    public const string Monthly = "Monthly";
    public const string Annual = "Annual";
}
