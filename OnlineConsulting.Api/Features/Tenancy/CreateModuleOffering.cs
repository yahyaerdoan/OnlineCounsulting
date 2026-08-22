using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.CreateModuleOffering;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class CreateModuleOffering : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/admin/module-offerings", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("CreateModuleOffering")
            .WithDescription("Creates a module offering (SuperAdmin) and its provider-side product/price.");
    }

    private static async Task<IResult> Handle([FromBody] CreateModuleOfferingCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
