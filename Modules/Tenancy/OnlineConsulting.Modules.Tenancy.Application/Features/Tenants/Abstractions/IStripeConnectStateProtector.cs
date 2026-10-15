namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;

/// <summary>Tamper-proof, short-lived carrier for the OAuth "state" parameter - Stripe's redirect back to the platform's fixed callback URL has no auth token, so this is how the callback recovers which tenant started the flow and where to send the browser back to.</summary>
public interface IStripeConnectStateProtector
{
    string Protect(Guid tenantId, string returnUrl);

    /// <summary>Null if the token is missing, tampered with, or expired.</summary>
    (Guid TenantId, string ReturnUrl)? Unprotect(string state);
}
