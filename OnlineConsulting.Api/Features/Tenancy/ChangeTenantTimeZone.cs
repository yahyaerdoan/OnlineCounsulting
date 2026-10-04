using Microsoft.AspNetCore.Mvc;
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

    private static async Task<IResult> Handle([FromBody] ChangeTenantTimeZoneRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record ChangeTenantTimeZoneRequest(string TimeZoneId)
{
    public ChangeTenantTimeZoneCommand ToCommand() => new(TimeZoneId);
}
