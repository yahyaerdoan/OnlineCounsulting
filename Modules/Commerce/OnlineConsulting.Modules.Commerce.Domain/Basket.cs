using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

public class Basket : TenantEntity<Guid>
{
    // Exactly one of UserId/GuestId is set - a basket belongs either to a logged-in user or to an
    // anonymous guest (identified by a long-lived cookie), never both. GuestId lets someone add
    // items before registering/logging in; the guest basket is merged into the user's basket at
    // login (see MergeGuestBasketCommand) and checkout always requires a UserId.
    // Plain ids, no navigation - User lives in the Identity module's own DbContext.
    public Guid? UserId { get; set; }
    public Guid? GuestId { get; set; }

    public int Quantity { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
