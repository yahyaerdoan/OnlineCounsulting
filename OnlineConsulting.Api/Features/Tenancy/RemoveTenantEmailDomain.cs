using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.RemoveTenantEmailDomain;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class RemoveTenantEmailDomain : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/tenancy/my-tenant/email-domain", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("RemoveTenantEmailDomain")
            .WithDescription("Removes the caller's email domain; emails go out from the platform address again. Tenant admins only.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new RemoveTenantEmailDomainCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}
