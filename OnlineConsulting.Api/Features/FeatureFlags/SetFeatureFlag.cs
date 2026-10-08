using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.FeatureFlags.SetFeatureFlag;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.FeatureFlags;

public class SetFeatureFlag : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/admin/feature-flags/{key}", Handle)
            .WithTags("FeatureFlags")
            .RequireAuthorization()
            .WithName("SetFeatureFlag")
            .WithDescription("Enables or disables a feature flag for the current tenant.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(
        string key, [FromBody] SetFeatureFlagRequest request, ISender sender, ITenantProvider tenantProvider, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(key, tenantProvider.TenantId));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record SetFeatureFlagRequest(bool IsEnabled)
{
    public SetFeatureFlagCommand ToCommand(string key, Guid tenantId) => new(key, IsEnabled) { TenantId = tenantId };
}
