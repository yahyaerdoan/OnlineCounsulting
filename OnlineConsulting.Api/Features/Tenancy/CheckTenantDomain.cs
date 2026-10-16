using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.CheckTenantDomain;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class CheckTenantDomain : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/my-tenant/domain/check", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("CheckTenantDomain")
            .WithDescription("Checks the caller's pending domain now and activates it when DNS and the certificate are ready. Tenant admins only.")
            .ProducesEnveloped<TenantDomainResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new CheckTenantDomainCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}
