using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

public class BasketItem : TenantEntity<Guid>
{
    public required Guid BasketId { get; set; }

    // Plain id, no navigation - Service lives in the not-yet-migrated legacy catalog, and modules
    // never reference each other's entities directly, only by id.
    public required Guid ServiceId { get; set; }

    public required int Quantity { get; set; }
    public required decimal Price { get; set; }
    public required int TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
