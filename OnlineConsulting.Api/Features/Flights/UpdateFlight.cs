using Microsoft.AspNetCore.Mvc;
using ResultHandler.AspNetCore.Extensions;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Concretions.Filters.ValidationFilters;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;
using System.Globalization;

namespace OnlineConsulting.Api.Features.Flights;

public class UpdateFlight : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/flights/{id}", HandleAsync)
            .AddEndpointFilter<ValidationEndpointFilter<UpdateFlightDto>>()
            .WithTags("Flights")
            .WithName("UpdateFlightTimeOrGate")
            .WithDescription("Updates a flight planned time and-or gate.")
            .WithPlaceholderExample(new UpdateFlightDto
            {
                PlannedDeparture = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture),
                PlannedArrival = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture),
                Gate = "A12"
            });
    }

    private static async Task<IResult> HandleAsync([FromRoute] Guid id, [FromBody] UpdateFlightDto updateFlightDto, IServiceManager serviceManager, HttpContext httpContext)
    {
        updateFlightDto.Id = id;
        var result = await serviceManager.FlightService.UpdateFlightDepartureAndGateAsync(updateFlightDto);
        return result.ToResult(httpContext);
    }
}
