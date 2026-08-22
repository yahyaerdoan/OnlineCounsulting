using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ReactivateTenant;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class ReactivateTenant : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tenancy/admin/tenants/{tenantId:guid}/reactivate", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("ReactivateTenant")
            .WithDescription("Lifts a suspension, restoring the tenant to Active (SuperAdmin).");
    }

    private static async Task<IResult> Handle(Guid tenantId, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new ReactivateTenantCommand(tenantId));
        return result.ToEnvelopedResult(httpContext);
    }
}
