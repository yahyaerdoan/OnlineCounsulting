using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Appointment;

public record AppointmentListItemViewModel(
    Guid Id,
    string CustomerName,
    string ServiceName,
    DateTimeOffset ScheduledStart,
    DateTimeOffset ScheduledEnd,
    string Status,
    string? AssignedTechnicianName,
    Guid? AssignedTechnicianUserId);

public record UserOptionViewModel(Guid Id, string Name);

public record EquipmentOptionViewModel(Guid Id, string Label);

public class AssignTechnicianViewModel
{
    public Guid AppointmentId { get; set; }

    [Required]
    public Guid TechnicianUserId { get; set; }

    public List<UserOptionViewModel> Technicians { get; set; } = [];
}

public class RecordWorkOrderViewModel
{
    public Guid AppointmentId { get; set; }

    /// <summary>Carried as a hidden field between GET and POST so RecordWorkOrderAsync doesn't need to
    /// re-look-up the appointment's owner when building the NewEquipment.CustomerUserId payload.</summary>
    public Guid CustomerUserId { get; set; }

    [Required]
    public Guid TechnicianUserId { get; set; }

    /// <summary>Null/empty means "use NewEquipment* fields instead" (a new equipment record gets created
    /// inline, mirroring the Api's CreateWorkOrderRequest.NewEquipment branch).</summary>
    public Guid? EquipmentId { get; set; }

    [MaxLength(100)]
    public string? NewEquipmentType { get; set; }

    [MaxLength(100)]
    public string? NewEquipmentBrand { get; set; }

    [MaxLength(100)]
    public string? NewEquipmentModel { get; set; }

    [MaxLength(100)]
    public string? NewEquipmentSerialNumber { get; set; }

    [MaxLength(2000)]
    public string? PartsUsed { get; set; }

    [MaxLength(2000)]
    public string? TechnicianNotes { get; set; }

    public IFormFile? BeforePhoto { get; set; }

    public IFormFile? AfterPhoto { get; set; }

    public List<UserOptionViewModel> Technicians { get; set; } = [];

    public List<EquipmentOptionViewModel> ExistingEquipment { get; set; } = [];
}

public class WorkOrderDetailViewModel
{
    public Guid AppointmentId { get; set; }
    public string? TechnicianName { get; set; }
    public string? EquipmentLabel { get; set; }
    public string? PartsUsed { get; set; }
    public string? TechnicianNotes { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? BeforePhotoUrl { get; set; }
    public string? AfterPhotoUrl { get; set; }
}
