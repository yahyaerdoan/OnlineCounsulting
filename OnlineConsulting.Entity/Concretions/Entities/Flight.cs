using OnlineConsulting.Entity.Concretions.BaseEntities;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Flight : BaseEntity
{
    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;

    public DateTime PlannedDeparture { get; set; }
    public DateTime PlannedArrival { get; set; }

    public DateTime? ActualDeparture { get; set; }
    public DateTime? ActualArrival { get; set; }
    public DateTime? CancelledAt { get; set; }

    public string? Gate { get; set; }
    public FlightStatus FlightStatus { get; set; }
}
public enum FlightStatus
{
    Scheduled,
    Boarding,
    Departed,
    Arrived,
    Cancelled,
    Delayed
}
