namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Marker for tenant-scoped entities - TenantEntityTypeBuilderExtensions.ApplyTenantAndSoftDeleteFilter enforces the global query filter for anything implementing this.</summary>
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}
