namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Placeholder tenant until real onboarding exists; all current rows use this id.</summary>
public static class TenantDefaults
{
    public static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
}
