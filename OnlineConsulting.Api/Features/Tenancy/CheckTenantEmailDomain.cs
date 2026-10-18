using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.CheckTenantEmailDomain;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Contracts;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class CheckTenantEmailDomain : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/my-tenant/email-domain/check", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("CheckTenantEmailDomain")
            .WithDescription("Verifies the caller's pending email domain now and activates it once its DNS records authenticate it. Tenant admins only.")
            .ProducesEnveloped<TenantEmailDomainResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new CheckTenantEmailDomainCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}
