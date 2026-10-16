namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read of the email a tenant signed up with; implemented by the Tenancy module.</summary>
public interface ITenantContactReader
{
    /// <summary>The tenant's signup email; null for the default tenant and unknown tenants.</summary>
    Task<string?> GetContactEmailAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
