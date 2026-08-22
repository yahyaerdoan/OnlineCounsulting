using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.SuspendTenant;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class SuspendTenant : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/admin/tenants/{tenantId:guid}/suspend", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("SuspendTenant")
            .WithDescription("Suspends a tenant, blocking its users from every protected endpoint (SuperAdmin).");
    }

    private static async Task<IResult> Handle(Guid tenantId, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new SuspendTenantCommand(tenantId));
        return result.ToEnvelopedResult(httpContext);
    }
}
