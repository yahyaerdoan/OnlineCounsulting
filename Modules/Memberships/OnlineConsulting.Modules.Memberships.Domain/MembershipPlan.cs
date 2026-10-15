using Core.PersistenceLayer.MultiTenancy;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Domain;

/// <summary>A membership plan for sale. Price, billing cycle and trial are fixed at creation, since provider prices are immutable.</summary>
public class MembershipPlan : SequentialGuidTenantEntity
{
    private MembershipPlan()
    {
    }

    public string Name { get; private set; } = string.Empty;

    /// <summary>One of <see cref="BillingCycles"/>.</summary>
    public string BillingCycle { get; private set; } = string.Empty;

    public decimal Price { get; private set; }
    public int IncludedVisitsPerYear { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal CreditAmount { get; private set; }
    public string? Benefits { get; private set; }

    /// <summary>Provider product and price, set once after creation.</summary>
    public string? ProviderProductId { get; private set; }
    public string? ProviderPriceId { get; private set; }

    /// <summary>False when retired from sale; existing members keep it.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Null means no trial.</summary>
    public int? TrialDays { get; private set; }

    public static MembershipPlan Create(string name, string billingCycle, decimal price, int includedVisitsPerYear, decimal discountPercent, decimal creditAmount,
        string? benefits, int? trialDays)
    {
        if (billingCycle is not (BillingCycles.Monthly or BillingCycles.Annual))
        {
            throw new ArgumentException($"Unknown billing cycle '{billingCycle}'.", nameof(billingCycle));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        if (trialDays is { } days)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days, nameof(trialDays));
        }

        var plan = new MembershipPlan { BillingCycle = billingCycle, Price = price, TrialDays = trialDays };
        plan.UpdateDetails(name, includedVisitsPerYear, discountPercent, creditAmount, benefits);
        return plan;
    }

    /// <summary>Changes what members get; never the price, billing cycle or trial.</summary>
    public void UpdateDetails(string name, int includedVisitsPerYear, decimal discountPercent, decimal creditAmount, string? benefits)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(includedVisitsPerYear);
        ArgumentOutOfRangeException.ThrowIfNegative(discountPercent);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(discountPercent, 100);
        ArgumentOutOfRangeException.ThrowIfNegative(creditAmount);

        Name = name;
        IncludedVisitsPerYear = includedVisitsPerYear;
        DiscountPercent = discountPercent;
        CreditAmount = creditAmount;
        Benefits = benefits;
    }

    /// <summary>Records the provider's product and price; only once.</summary>
    public void AttachProviderPrice(string providerProductId, string providerPriceId)
    {
        if (ProviderPriceId is not null)
        {
            throw new InvalidOperationException($"Membership plan {Id} already has provider price {ProviderPriceId}.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(providerProductId);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerPriceId);
        ProviderProductId = providerProductId;
        ProviderPriceId = providerPriceId;
    }

    /// <summary>Puts the plan on sale (true) or retires it (false).</summary>
    public void SetActive(bool isActive) => IsActive = isActive;
}
