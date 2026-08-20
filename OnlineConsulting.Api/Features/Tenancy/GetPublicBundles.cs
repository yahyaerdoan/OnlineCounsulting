using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.GetPublicBundles;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetPublicBundles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/tenancy/bundles", Handle)
            .WithTags("Tenancy")
            .WithName("GetPublicBundles")
            .WithDescription("Returns every publicly visible bundle - pricing-page shortcut data source for the signup form.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetPublicBundlesQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
