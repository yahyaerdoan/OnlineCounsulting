using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

public class Order : TenantEntity<Guid>
{
    public required string OrderNumber { get; set; }
    public required string OrderStatus { get; set; }
    public required string PaymentStatus { get; set; }

    // Plain id, no navigation - User lives in the Identity module's own DbContext.
    public required Guid UserId { get; set; }

    // Plain ids, no navigation - UserAddress lives in this same module/DbContext (Addresses group)
    // but repositories here compose by id, the same way every other slice in this module does.
    public required Guid ShippingAddressId { get; set; }
    public required Guid InvoiceAddressId { get; set; }
}
