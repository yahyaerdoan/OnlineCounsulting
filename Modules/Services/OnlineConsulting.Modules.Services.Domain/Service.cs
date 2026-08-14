using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Services.Domain;

public class Service : TenantEntity<Guid>
{
    // Plain id, no navigation - Category lives in the Categories module's own DbContext, modules
    // never reference each other's entities directly, only by id.
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
}
