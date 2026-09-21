using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>TenantId rides in the route rather than the caller's JWT claim, since a SuperAdmin may act on another tenant; AddModuleCommand enforces ownership via TenantOwnershipGuard.</summary>
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
