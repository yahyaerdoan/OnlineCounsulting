using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantBranding;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetTenantBranding : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tenancy/branding", Handle)
            .WithTags("Tenancy")
            .WithName("GetTenantBranding")
            .WithDescription("Returns the business name and logo the caller's site shows (the platform's own for the default tenant). Public.")
            .ProducesEnveloped<TenantBrandingResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetTenantBrandingQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
