namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Builds a tenant's own site origin for links in emails; implemented by the Tenancy module.</summary>
public interface ITenantOriginReader
{
    /// <summary>The tenant's site origin without a trailing slash, e.g. "https://acme.example.com".</summary>
    Task<string> GetOriginAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
