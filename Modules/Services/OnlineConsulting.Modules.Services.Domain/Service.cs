using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Services.Domain;

public class Service : TenantEntity<Guid>
{
    /// <summary>Plain id, no navigation, since modules never reference each other's entities directly, only by id.</summary>
    public required Guid CategoryId { get; set; }

    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public required string DetailedDescription { get; set; }
    public required decimal Price { get; set; }

    /// <summary>Application.Features.Constants.ServicePriceTypes.* - Fixed shows Price as-is, StartingAt shows "From {Price}" (a diagnostic/call-out fee, actual repair cost varies), Range shows "{Price} - {PriceMax}". Honest pricing display instead of a single number that's misleading for variable-cost repair work.</summary>
    public string PriceType { get; set; } = "Fixed";

    /// <summary>Only meaningful when PriceType is Range.</summary>
    public decimal? PriceMax { get; set; }

    public bool FeaturedArea { get; set; }
    public int DiscountRate { get; set; }
    public int TaxRate { get; set; }
    public decimal DiscountedPrice { get; set; }

    /// <summary>When true, a Scheduling appointment for this service must reach PendingPayment/Confirmed via a paid Commerce order before the tenant confirms it. False (default) keeps booking payment-free until a real payment gateway exists.</summary>
    public bool RequiresPrepayment { get; set; }

    /// <summary>Whether this service can be requested as an urgent/24-7 callout (e.g. "HVAC Emergency"), not a separate service - an urgency modifier on the same service.</summary>
    public bool IsEmergencyAvailable { get; set; }

    /// <summary>Plain id, no navigation - MediaAsset lives in the Media module. Null means no cover photo uploaded yet.</summary>
    public Guid? CoverMediaAssetId { get; set; }
}
