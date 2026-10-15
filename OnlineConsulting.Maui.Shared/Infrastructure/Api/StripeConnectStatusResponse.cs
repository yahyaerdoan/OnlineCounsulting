using OnlineConsulting.Maui.Shared.Infrastructure.Hateoas;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/tenancy/my-tenant/stripe-connect/refresh.</summary>
public record StripeConnectStatusResponse(bool IsConnected, bool ChargesEnabled, bool PayoutsEnabled) : HalResource;
