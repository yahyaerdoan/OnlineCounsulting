using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.AvailabilityRule;

public record AvailabilityRuleListItemViewModel(Guid Id, DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int SlotDurationMinutes);

public class CreateAvailabilityRuleViewModel
{
    [Required]
    public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;

    [Required]
    public string StartTime { get; set; } = "09:00";

    [Required]
    public string EndTime { get; set; } = "17:00";

    [Range(5, 480)]
    public int SlotDurationMinutes { get; set; } = 60;
}
