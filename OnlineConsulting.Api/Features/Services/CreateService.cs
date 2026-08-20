using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.CreateService;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class CreateService : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/services", Handle)
            .WithTags("Services")
            .RequireAuthorization()
            .WithName("CreateService")
            .WithDescription("Creates a new service in the catalog.");
    }

    private static async Task<IResult> Handle([FromBody] CreateServiceCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
