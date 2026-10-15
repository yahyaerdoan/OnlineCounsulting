using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants;

namespace OnlineConsulting.Api.Features.Payments;

/// <summary>Where Stripe redirects the browser after the tenant admin finishes (or cancels) the Connect Standard OAuth consent screen. Fixed, version-neutral
/// path - it must exactly match the redirect_uri registered in the Stripe dashboard's Connect OAuth settings, so it can't move with API versioning.</summary>
public class StripeConnectOAuthCallback : IVersionNeutralEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(StripeConnectOAuthRoutes.CallbackPath, Handle)
            .WithTags("Payments")
            .WithName("StripeConnectOAuthCallback")
            .WithDescription("Exchanges the Stripe Connect OAuth code for the tenant's own connected account id, then redirects back to the tenant's onboarding return page.")
            .Produces(StatusCodes.Status302Found);
    }

    private static async Task<IResult> Handle(string? code, string? state, StripeConnectOAuthCallbackHandler handler, CancellationToken cancellationToken)
    {
        var returnUrl = await handler.HandleAsync(code, state, cancellationToken);

        return returnUrl is null ? Results.BadRequest("This Stripe Connect link is invalid or has expired.") : Results.Redirect(returnUrl);
    }
}
