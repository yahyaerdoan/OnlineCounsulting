using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/tenancy/my-tenant/stripe-connect/onboarding-link.</summary>
public record StripeConnectOnboardingLinkResponse(string Url) : HalResource;
