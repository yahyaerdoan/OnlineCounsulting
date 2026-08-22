using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.GetBundleById;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetBundleById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/tenancy/admin/bundles/{id:guid}", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetBundleById")
            .WithDescription("Returns a single bundle, including hidden ones (SuperAdmin).");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetBundleByIdQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
