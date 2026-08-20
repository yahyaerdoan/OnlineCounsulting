using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.FeatureFlags;

public class SetFeatureFlag : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/admin/feature-flags/{key}", Handle)
            .WithTags("FeatureFlags")
            .RequireAuthorization()
            .WithName("SetFeatureFlag")
            .WithDescription("Enables or disables a feature flag for the current tenant.");
    }

    private static async Task<IResult> Handle(
        string key, [FromBody] SetFeatureFlagCommand command, ISender sender, ITenantProvider tenantProvider, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Key = key, TenantId = tenantProvider.TenantId });
        return result.ToEnvelopedResult(httpContext);
    }
}
