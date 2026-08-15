using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

public class Basket : TenantEntity<Guid>
{
    /// <summary>Exactly one of UserId/GuestId is set - a basket belongs either to a logged-in user or to an anonymous guest, never both.</summary>
    public Guid? UserId { get; set; }
    public Guid? GuestId { get; set; }

    public int Quantity { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
