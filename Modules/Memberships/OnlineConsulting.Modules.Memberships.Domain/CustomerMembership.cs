using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Domain;

public class CustomerMembership : TenantEntity<Guid>
{
    /// <summary>Plain id, no navigation - User lives in the Identity module's own DbContext.</summary>
    public required Guid UserId { get; set; }

    /// <summary>Plain id, no navigation, same cross-module convention as Appointment.ServiceId.</summary>
    public required Guid MembershipPlanId { get; set; }

    public required string Status { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? RenewalDate { get; set; }

    public string? ProviderCustomerId { get; set; }
    public string? ProviderSubscriptionId { get; set; }
}
