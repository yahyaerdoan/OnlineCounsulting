using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

/// <summary>Removes one module from a tenant's subscription (post-signup). Same TenantId-in-route reasoning as AddModule.cs.</summary>
public class RemoveModule : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/tenancy/{tenantId:guid}/modules/{key}", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("RemoveModule")
            .WithDescription("Removes a module from a tenant's subscription, prorated refund/credit for the remainder of the current period.");
    }

    private static async Task<IResult> Handle(Guid tenantId, string key, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new RemoveModuleCommand(tenantId, key));
        return result.ToEnvelopedResult(httpContext);
    }
}
