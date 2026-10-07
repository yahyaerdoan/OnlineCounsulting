namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>The platform owner's own tenant: the SuperAdmin and the platform site (signup, pricing) live here.</summary>
public static class TenantDefaults
{
    public static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
}
