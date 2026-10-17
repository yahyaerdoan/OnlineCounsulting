namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>How a mid-period subscription change is billed.</summary>
public enum ProrationBilling
{
    /// <summary>Invoice and charge the prorated amount right away; fails if the payment fails.</summary>
    ChargeNow,

    /// <summary>No proration: nothing charged or credited for the current period.</summary>
    None,
}
