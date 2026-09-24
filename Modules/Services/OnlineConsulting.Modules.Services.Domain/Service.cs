using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Services.Domain;

public class Service : SequentialGuidTenantEntity
{
    /// <summary>Plain id, no navigation, since modules never reference each other's entities directly, only by id.</summary>
    public required Guid CategoryId { get; set; }

    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public required string DetailedDescription { get; set; }
    public required decimal Price { get; set; }

    /// <summary>ServicePriceTypes.* - Fixed shows Price as-is, StartingAt shows "From {Price}", Range shows "{Price} - {PriceMax}".</summary>
    public string PriceType { get; set; } = "Fixed";

    /// <summary>Only meaningful when PriceType is Range.</summary>
    public decimal? PriceMax { get; set; }

    public bool FeaturedArea { get; set; }
    public int DiscountRate { get; set; }
    public int TaxRate { get; set; }
    public decimal DiscountedPrice { get; set; }

    /// <summary>When true, a Scheduling appointment must reach PendingPayment/Confirmed via a paid Commerce order before the tenant confirms it.</summary>
    public bool RequiresPrepayment { get; set; }

    /// <summary>Whether this service can be requested as an urgent/24-7 callout (e.g. "HVAC Emergency"), not a separate service - an urgency modifier on the same service.</summary>
    public bool IsEmergencyAvailable { get; set; }

    /// <summary>Plain id, no navigation - MediaAsset lives in the Media module. Null means no cover photo uploaded yet.</summary>
    public Guid? CoverMediaAssetId { get; set; }
}
