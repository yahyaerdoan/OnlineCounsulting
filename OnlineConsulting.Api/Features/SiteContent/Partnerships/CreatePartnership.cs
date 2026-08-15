using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.CreatePartnership;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Partnerships;

public class CreatePartnership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/site-content/partnerships", Handle)
            .WithTags("SiteContent/Partnerships")
            .RequireAuthorization()
            .WithName("CreatePartnership")
            .WithDescription("Creates a partnership showcase entry.");
    }

    private static async Task<IResult> Handle([FromBody] CreatePartnershipCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
