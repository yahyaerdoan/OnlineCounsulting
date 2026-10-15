using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.RefreshStripeConnectStatus;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class RefreshStripeConnectStatus : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tenancy/my-tenant/stripe-connect/refresh", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("RefreshStripeConnectStatus")
            .WithDescription("Pulls the caller's own tenant's Connect account capabilities live from the provider - lets the onboarding return page show status without waiting for the webhook. Tenant admins only.")
            .ProducesEnveloped<StripeConnectStatusResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new RefreshStripeConnectStatusCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}
