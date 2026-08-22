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
        _ = app.MapPut("/api/tenancy/admin/bundles/{id:guid}", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("UpdateBundle")
            .WithDescription("Updates a bundle's name, module keys and visibility (SuperAdmin).");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateBundleCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
