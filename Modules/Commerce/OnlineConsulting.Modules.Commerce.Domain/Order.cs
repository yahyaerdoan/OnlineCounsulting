using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

public class Order : TenantEntity<Guid>
{
    public required string OrderNumber { get; set; }
    public required string OrderStatus { get; set; }
    public required string PaymentStatus { get; set; }

    /// <summary>Which IPaymentGateway processed this order (PaymentProviderNames.*) - a refund must route back to the provider that actually took the payment, not whichever provider is active when the refund happens.</summary>
    public string? PaymentProvider { get; set; }
    public string? ProviderPaymentId { get; set; }

    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext.</summary>
    public required Guid UserId { get; set; }

    /// <summary>Plain id, no navigation - UserAddress is composed by id here, same as every other slice in this module.</summary>
    public required Guid ShippingAddressId { get; set; }
    public required Guid InvoiceAddressId { get; set; }
}
