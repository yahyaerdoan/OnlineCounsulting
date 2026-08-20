using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Adds one more à la carte module to a tenant's subscription (post-signup). TenantId rides in the route, not derived from the caller's own JWT tenant claim, because the caller may be a SuperAdmin acting on a tenant other than their own - AddModuleCommand itself enforces "caller's own tenant, or SuperAdmin" (see TenantOwnershipGuard).</summary>
public class AddModule : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/{tenantId:guid}/modules/{key}", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("AddModule")
            .WithDescription("Adds one more module to a tenant's subscription, billed immediately and prorated.");
    }

    private static async Task<IResult> Handle(Guid tenantId, string key, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new AddModuleCommand(tenantId, key));
        return result.ToEnvelopedResult(httpContext);
    }
}
