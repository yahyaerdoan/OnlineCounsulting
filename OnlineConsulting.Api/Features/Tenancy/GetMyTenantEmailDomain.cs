using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.GetMyTenantEmailDomain;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetMyTenantEmailDomain : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tenancy/my-tenant/email-domain", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetMyTenantEmailDomain")
            .WithDescription("Returns the caller's own email sending domain and, while pending, the DNS records to add. Tenant admins only.")
            .ProducesEnveloped<TenantEmailDomainResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetMyTenantEmailDomainQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
