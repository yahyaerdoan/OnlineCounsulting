namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Scheduling's AvailableSlotResponse.</summary>
public record AvailableSlotResponse(DateTimeOffset Start, DateTimeOffset End);
