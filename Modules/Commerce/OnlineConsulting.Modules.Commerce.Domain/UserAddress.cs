using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

public class UserAddress : TenantEntity<Guid>
{
    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext, modules only reference each other by id.</summary>
    public required Guid UserId { get; set; }

    public required string AddressName { get; set; }
    public string? CompanyName { get; set; }
    public required string Country { get; set; }
    public required string AddressLine { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Zipcode { get; set; }
    public string? Notes { get; set; }
    public bool IsShippingAddress { get; set; }
    public bool IsBillingAddress { get; set; }
}
