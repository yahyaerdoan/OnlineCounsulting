using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.GetBundles;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetBundles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tenancy/admin/bundles", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetBundles")
            .WithDescription("Returns every bundle, including hidden ones (SuperAdmin).")
            .ProducesEnveloped<Paginate<BundleAdminResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetBundlesQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
