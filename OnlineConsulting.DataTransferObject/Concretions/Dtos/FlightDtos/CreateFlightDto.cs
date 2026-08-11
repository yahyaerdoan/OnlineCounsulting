using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;

public class CreateFlightDto : IDto
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;

    public string PlannedDeparture { get; set; } = string.Empty;
    public string PlannedArrival { get; set; } = string.Empty;

    public DateTime? ActualDeparture { get; set; }
    public DateTime? ActualArrival { get; set; }
    public DateTime? CancelledAt { get; set; }

    public string? Gate { get; set; }
}
