using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.GetModuleOfferings;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetModuleOfferings : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/tenancy/admin/module-offerings", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetModuleOfferings")
            .WithDescription("Returns every module offering, including hidden ones (SuperAdmin).");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetModuleOfferingsQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
