namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

/// <summary>Single-use, short-lived - the caller redirects to Url immediately, never stores it.</summary>
public record StripeConnectOnboardingLinkResponse(string Url);
