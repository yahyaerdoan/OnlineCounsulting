namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Contracts;

public record AvailableSlotResponse(DateTimeOffset Start, DateTimeOffset End);
