using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Equipment.Domain;

/// <summary>An installed HVAC system owned by a customer.</summary>
public class EquipmentItem : SequentialGuidTenantEntity
{
    public required Guid UserId { get; set; }
    public required string Type { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public DateTimeOffset? InstallDate { get; set; }
    public DateTimeOffset? WarrantyExpiresAt { get; set; }
    public string? Notes { get; set; }
}
