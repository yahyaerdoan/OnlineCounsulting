using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.UpdateBundle;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class UpdateBundle : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/tenancy/admin/bundles/{id:guid}", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("UpdateBundle")
            .WithDescription("Updates a bundle's name, module keys and visibility (SuperAdmin).");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateBundleRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateBundleRequest(string Name, List<string> ModuleKeys, bool IsPubliclyVisible)
{
    public UpdateBundleCommand ToCommand(Guid id) => new(id, Name, ModuleKeys, IsPubliclyVisible);
}
