namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Maps the host a client is serving (a tenant's site or app) to its tenant; implemented by the Tenancy module.</summary>
public interface ITenantHostResolver
{
    /// <summary>The tenant behind <paramref name="host"/> (no scheme or port), or null when no tenant owns it.</summary>
    Task<Guid?> ResolveAsync(string host, CancellationToken cancellationToken = default);
}
