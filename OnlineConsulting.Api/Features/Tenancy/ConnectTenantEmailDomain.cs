using Microsoft.AspNetCore.Mvc;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.ConnectTenantEmailDomain;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Contracts;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class ConnectTenantEmailDomain : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/my-tenant/email-domain", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("ConnectTenantEmailDomain")
            .WithDescription("Registers the caller's email domain (e.g. \"example.com\") with the email provider and returns the DNS records to add. Tenant admins only.")
            .ProducesEnveloped<TenantEmailDomainResponse>();
    }

    private static async Task<IResult> Handle([FromBody] ConnectTenantEmailDomainRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record ConnectTenantEmailDomainRequest(string Domain)
{
    public ConnectTenantEmailDomainCommand ToCommand() => new(Domain);
}
