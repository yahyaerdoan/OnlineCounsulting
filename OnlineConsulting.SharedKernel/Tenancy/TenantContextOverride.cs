namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Ambient override for ITenantProvider.TenantId, scoped to the current async call chain via AsyncLocal. TenantProvider checks this before falling back to the HTTP request's JWT claim, so code with no "current" tenant of its own (background jobs, webhook handlers, a SuperAdmin acting on a tenant that isn't their own) can direct reads/writes at an explicit tenant without an HTTP context - see IFeatureFlagWriter's implementation for the intended usage.</summary>
public static class TenantContextOverride
{
    private static readonly AsyncLocal<Guid?> Current = new();

    public static Guid? TenantId => Current.Value;

    public static IDisposable BeginScope(Guid tenantId)
    {
        var previous = Current.Value;
        Current.Value = tenantId;
        return new Scope(previous);
    }

    private sealed class Scope(Guid? previousTenantId) : IDisposable
    {
        public void Dispose() => Current.Value = previousTenantId;
    }
}
