using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.CreateBundle;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class CreateBundle : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/admin/bundles", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("CreateBundle")
            .WithDescription("Creates a bundle - a shortcut group of existing module offerings (SuperAdmin).");
    }

    private static async Task<IResult> Handle([FromBody] CreateBundleCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
