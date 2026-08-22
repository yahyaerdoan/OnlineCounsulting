using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantById;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetTenantById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/tenancy/admin/tenants/{tenantId:guid}", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetTenantById")
            .WithDescription("Returns a single tenant's detail: subscription and every subscription item ever billed on it (SuperAdmin).");
    }

    private static async Task<IResult> Handle(Guid tenantId, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetTenantByIdQuery(tenantId));
        return result.ToEnvelopedResult(httpContext);
    }
}
