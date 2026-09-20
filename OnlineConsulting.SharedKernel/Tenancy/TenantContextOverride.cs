namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>AsyncLocal override for ITenantProvider.TenantId, checked before the JWT claim, so code with no HTTP context (background jobs, webhooks) can direct reads/writes at an explicit tenant.</summary>
public static class TenantContextOverride
{
    private static readonly AsyncLocal<Guid?> _current = new();

    public static Guid? TenantId => _current.Value;

    public static IDisposable BeginScope(Guid tenantId)
    {
        var previous = _current.Value;
        _current.Value = tenantId;

        return new Scope(previous);
    }

    private sealed class Scope(Guid? previousTenantId) : IDisposable
    {
        public void Dispose() => _current.Value = previousTenantId;
    }
}
