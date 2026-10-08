using Hateoas.AspNetCore;
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
        _ = app.MapPost("/tenancy/admin/bundles", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("CreateBundle")
            .WithCreatedLocation("GetBundleById")
            .WithDescription("Creates a bundle - a shortcut group of existing module offerings (SuperAdmin).")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateBundleRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateBundleRequest(string Name, List<string> ModuleKeys, bool IsPubliclyVisible)
{
    public CreateBundleCommand ToCommand() => new(Name, ModuleKeys, IsPubliclyVisible);
}
