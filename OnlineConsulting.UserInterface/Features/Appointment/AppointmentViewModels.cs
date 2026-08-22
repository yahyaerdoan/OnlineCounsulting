using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Features.Appointment;

public record ServiceOptionViewModel(Guid Id, string Title);

public record AvailableSlotViewModel(DateTimeOffset Start, DateTimeOffset End);

public class BookAppointmentViewModel
{
    public Guid? ServiceId { get; set; }

    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public DateTimeOffset? SelectedSlotStart { get; set; }

    public DateTimeOffset? SelectedSlotEnd { get; set; }

    [MaxLength(1000)]
    public string? CustomerNote { get; set; }

    [MaxLength(300)]
    public string? ServiceAddress { get; set; }

    public List<ServiceOptionViewModel> Services { get; set; } = [];

    public List<AvailableSlotViewModel> AvailableSlots { get; set; } = [];
}
