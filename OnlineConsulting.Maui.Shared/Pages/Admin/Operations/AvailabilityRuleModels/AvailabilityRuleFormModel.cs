namespace OnlineConsulting.Maui.Shared.Pages.Admin.Operations.AvailabilityRuleModels;

public class AvailabilityRuleFormModel
{
    public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public int SlotDurationMinutes { get; set; } = 30;
}
