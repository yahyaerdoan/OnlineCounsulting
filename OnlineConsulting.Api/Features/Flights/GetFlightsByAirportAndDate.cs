using Microsoft.AspNetCore.Mvc;
using ResultHandler.AspNetCore.Extensions;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;

namespace OnlineConsulting.Api.Features.Flights;

public class GetFlightsByAirportAndDate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/flights/{airportCode}/{date}", HandleAsync)
            .WithTags("Flights")
            .WithName("GetFlightsByAirportAndDate")
            .WithDescription("Returns flights by airport code and date. Sorted by planned departure. Automatically compute status.")
            .WithParamExamples(new { airportCode = "ORD", date = "2025-08-19" });
    }

    private static async Task<IResult> HandleAsync([FromRoute] string airportCode, [FromRoute] DateTime date, IServiceManager serviceManager, HttpContext httpContext)
    {
        var result = await serviceManager.FlightService.GetFlightsByAirportGivenDayAsync<ResultFlightDto>(airportCode, date, false, true);
        return result.ToResult(httpContext);
    }
}
