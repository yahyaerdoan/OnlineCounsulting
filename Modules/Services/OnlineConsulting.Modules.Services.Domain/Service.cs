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
    public bool FeaturedArea { get; set; }
    public int DiscountRate { get; set; }
    public int TaxRate { get; set; }
    public decimal DiscountedPrice { get; set; }

    /// <summary>When true, a Scheduling appointment for this service must reach PendingPayment/Confirmed via a paid Commerce order before the tenant confirms it. False (default) keeps booking payment-free until a real payment gateway exists.</summary>
    public bool RequiresPrepayment { get; set; }
}
