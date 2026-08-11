using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using System.Text.Json.Serialization;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;

public class UpdateFlightDto : IDto
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string? PlannedDeparture { get; set; }
    public string? PlannedArrival { get; set; }
    public string? Gate { get; set; }
}
