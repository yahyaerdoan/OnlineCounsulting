using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ChangeTenantTimeZone;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class ChangeTenantTimeZone : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/tenancy/my-tenant/time-zone", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("ChangeTenantTimeZone")
            .WithDescription("Sets the caller's own business time zone (IANA id, e.g. \"America/Chicago\"). Tenant admins only.");
    }

    private static async Task<IResult> Handle(ChangeTenantTimeZoneCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
