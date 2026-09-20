using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.CancelTenant;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class CancelTenant : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/admin/tenants/{tenantId:guid}/cancel", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("CancelTenant")
            .WithDescription("Permanently cancels a tenant's subscription with the payment provider and marks it Cancelled - irreversible, unlike Suspend (SuperAdmin).");
    }

    private static async Task<IResult> Handle(Guid tenantId, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new CancelTenantCommand(tenantId));
        return result.ToEnvelopedResult(httpContext);
    }
}
