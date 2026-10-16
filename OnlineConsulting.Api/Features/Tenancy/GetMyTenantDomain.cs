using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.GetMyTenantDomain;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetMyTenantDomain : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tenancy/my-tenant/domain", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetMyTenantDomain")
            .WithDescription("Returns the caller's own custom domain and, while pending, the DNS records to add. Tenant admins only.")
            .ProducesEnveloped<TenantDomainResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetMyTenantDomainQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
