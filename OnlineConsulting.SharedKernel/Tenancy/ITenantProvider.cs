namespace OnlineConsulting.SharedKernel.Tenancy;

public interface ITenantProvider
{
    Guid TenantId { get; }
}
