namespace OnlineConsulting.Maui.Shared.Pages.Admin.Operations.EquipmentModels;

public class EquipmentFormModel
{
    /// <summary>Create only - ownership is fixed at creation, never changes on edit. Nullable so the
    /// customer picker starts unselected instead of defaulting to a misleading first customer.</summary>
    public Guid? UserId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public string? SerialNumber { get; set; }

    public DateTime? InstallDate { get; set; }

    public DateTime? WarrantyExpiresAt { get; set; }

    public string? Notes { get; set; }
}
