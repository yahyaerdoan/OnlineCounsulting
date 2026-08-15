namespace OnlineConsulting.Modules.Scheduling.Application.Contracts;

public record AvailableSlotResponse(DateTimeOffset Start, DateTimeOffset End);
