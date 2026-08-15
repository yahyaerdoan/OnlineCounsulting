using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.GetFeatureFlags;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.FeatureFlags;

public class GetFeatureFlags : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/feature-flags", Handle)
            .WithTags("FeatureFlags")
            .RequireAuthorization()
            .WithName("GetFeatureFlags")
            .WithDescription("Returns every known feature flag key and whether it's enabled for the current tenant.");
    }

    private static async Task<IResult> Handle(ISender sender, ITenantProvider tenantProvider, HttpContext httpContext)
    {
        var result = await sender.Send(new GetFeatureFlagsQuery(tenantProvider.TenantId));
        return result.ToEnvelopedResult(httpContext);
    }
}
