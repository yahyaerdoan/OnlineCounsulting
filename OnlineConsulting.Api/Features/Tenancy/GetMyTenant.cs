using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetMyTenant;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetMyTenant : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/tenancy/my-tenant", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetMyTenant")
            .WithDescription("Returns the caller's own tenant: name, status, and active modules - self-service counterpart to the SuperAdmin-only GetTenantById.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetMyTenantQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
