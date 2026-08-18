using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Equipment.Domain;

/// <summary>An installed HVAC system owned by a customer (furnace, AC, heat pump, water heater...) - the customer portal's equipment health panel. Plain UserId, no navigation - User lives in the Identity module's own DbContext. Named EquipmentItem, not Equipment, to avoid colliding with the OnlineConsulting.Modules.Equipment namespace segment.</summary>
public class EquipmentItem : TenantEntity<Guid>
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
