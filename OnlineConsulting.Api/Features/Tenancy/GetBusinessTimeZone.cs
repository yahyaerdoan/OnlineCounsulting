using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetBusinessTimeZone;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetBusinessTimeZone : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/tenancy/time-zone", Handle)
            .WithTags("Tenancy")
            .WithName("GetBusinessTimeZone")
            .WithDescription("Returns the IANA time zone the caller's business runs in (the default tenant's for anonymous callers). Clients show every date and time in it. Public.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetBusinessTimeZoneQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
