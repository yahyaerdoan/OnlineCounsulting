using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.UpdateService;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class UpdateService : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/services/{id:guid}", Handle)
            .WithTags("Services")
            .RequireAuthorization()
            .WithName("UpdateService")
            .WithDescription("Updates an existing service.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateServiceCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
