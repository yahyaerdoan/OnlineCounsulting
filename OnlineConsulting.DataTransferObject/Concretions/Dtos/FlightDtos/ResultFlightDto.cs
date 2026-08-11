using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;

public class ResultFlightDto : IDto
{
    public Guid Id { get; set; }

    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;

    public DateTime PlannedDeparture { get; set; }
    public DateTime PlannedArrival { get; set; }

    public DateTime? ActualDeparture { get; set; }
    public DateTime? ActualArrival { get; set; }
    public DateTime? CancelledAt { get; set; }

    public string? Gate { get; set; }
    public string FlightStatus { get; set; } = string.Empty;
}
