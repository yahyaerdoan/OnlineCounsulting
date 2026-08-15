using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

public class OrderItem : TenantEntity<Guid>
{
    public required Guid OrderId { get; set; }

    /// <summary>Plain id, no navigation - Service lives in the not-yet-migrated legacy catalog.</summary>
    public required Guid ServiceId { get; set; }

    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
