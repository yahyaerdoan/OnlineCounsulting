namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants;

/// <summary>CallbackPath must exactly match the redirect_uri registered in the Stripe dashboard's Connect OAuth settings - version-neutral and fixed, unlike the onboarding-link request that carries the tenant's own return URL in the OAuth "state" instead.</summary>
public static class StripeConnectOAuthRoutes
{
    public const string CallbackPath = "/api/payments/stripe-connect/callback";
}
