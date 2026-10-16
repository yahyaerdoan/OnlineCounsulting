using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.RemoveTenantDomain;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class RemoveTenantDomain : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/tenancy/my-tenant/domain", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("RemoveTenantDomain")
            .WithDescription("Removes the caller's own custom domain; the site stays on its platform subdomain. Tenant admins only.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new RemoveTenantDomainCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}
