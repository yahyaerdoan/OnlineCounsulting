namespace OnlineConsulting.Maui.Shared.Pages.Admin.Operations.OperationsModels;

/// <summary>Backs RecordWorkOrderPage's EditForm.</summary>
public class RecordWorkOrderFormModel
{
    public Guid TechnicianUserId { get; set; }

    public string EquipmentMode { get; set; } = "existing";

    public Guid? EquipmentId { get; set; }

    public string NewEquipmentType { get; set; } = string.Empty;

    public string? NewEquipmentSerialNumber { get; set; }

    public string? NewEquipmentBrand { get; set; }

    public string? NewEquipmentModel { get; set; }

    public string? PartsUsed { get; set; }

    public string? TechnicianNotes { get; set; }
}
