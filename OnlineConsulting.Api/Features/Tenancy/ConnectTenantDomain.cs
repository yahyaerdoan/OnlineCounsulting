using Microsoft.AspNetCore.Mvc;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.ConnectTenantDomain;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class ConnectTenantDomain : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/my-tenant/domain", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("ConnectTenantDomain")
            .WithDescription("Registers the caller's own domain (e.g. \"www.example.com\") with the hosting provider and returns the DNS records to add. Tenant admins only.")
            .ProducesEnveloped<TenantDomainResponse>();
    }

    private static async Task<IResult> Handle([FromBody] ConnectTenantDomainRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record ConnectTenantDomainRequest(string Host)
{
    public ConnectTenantDomainCommand ToCommand() => new(Host);
}
