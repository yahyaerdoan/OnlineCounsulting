using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.GetPublicModuleOfferings;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetPublicModuleOfferings : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tenancy/module-offerings", Handle)
            .WithTags("Tenancy")
            .WithName("GetPublicModuleOfferings")
            .WithDescription("Returns every publicly visible module offering - pricing-page data source for the signup form.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetPublicModuleOfferingsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
