using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.UpdateTenantBranding;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class UpdateTenantBranding : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/tenancy/my-tenant/branding", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("UpdateTenantBranding")
            .WithDescription("Sets the caller's own business name and logo, shown on its site, app and emails. Tenant admins only.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle([FromBody] UpdateTenantBrandingRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateTenantBrandingRequest(string Name, Guid? LogoMediaAssetId)
{
    public UpdateTenantBrandingCommand ToCommand() => new(Name, LogoMediaAssetId);
}
