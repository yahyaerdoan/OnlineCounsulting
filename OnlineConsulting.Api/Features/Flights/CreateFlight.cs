using Microsoft.AspNetCore.Mvc;
using ResultHandler.AspNetCore.Extensions;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Concretions.Filters.ValidationFilters;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;
using System.Globalization;

namespace OnlineConsulting.Api.Features.Flights;

public class CreateFlight : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/flights", HandleAsync)
            .AddEndpointFilter<ValidationEndpointFilter<CreateFlightDto>>()
            .WithTags("Flights")
            .WithName("CreateFlightsToSchedule")
            .WithDescription("Create a flight in the schedule.")
            .WithPlaceholderExample(new CreateFlightDto
            {
                Origin = "ORD",
                Destination = "LAX",
                PlannedDeparture = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture),
                PlannedArrival = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture),
                ActualDeparture = null,
                ActualArrival = null,
                CancelledAt = null,
                Gate = "A12"
            });
    }

    private static async Task<IResult> HandleAsync([FromBody] CreateFlightDto flightDto, IServiceManager serviceManager, HttpContext httpContext)
    {
        var result = await serviceManager.FlightService.AddFlightToScheduleAsync(flightDto);
        return result.ToResult(httpContext);
    }
}
