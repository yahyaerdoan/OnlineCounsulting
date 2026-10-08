using Hateoas.AspNetCore;
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
        _ = app.MapPost("/tenancy/admin/module-offerings", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("CreateModuleOffering")
            .WithCreatedLocation("GetModuleOfferingById")
            .WithDescription("Creates a module offering (SuperAdmin) and its provider-side product/price.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateModuleOfferingRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateModuleOfferingRequest(string Key, string Name, decimal Price, string BillingCycle, bool IsPubliclyVisible)
{
    public CreateModuleOfferingCommand ToCommand() => new(Key, Name, Price, BillingCycle, IsPubliclyVisible);
}
