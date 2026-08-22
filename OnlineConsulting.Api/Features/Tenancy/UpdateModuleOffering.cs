using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.UpdateModuleOffering;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class UpdateModuleOffering : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/tenancy/admin/module-offerings/{id:guid}", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("UpdateModuleOffering")
            .WithDescription("Updates a module offering's local fields (SuperAdmin). Never changes the provider-side price.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateModuleOfferingCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
