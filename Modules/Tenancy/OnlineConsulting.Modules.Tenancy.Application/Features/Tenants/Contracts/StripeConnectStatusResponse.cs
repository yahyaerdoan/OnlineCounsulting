namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

public record StripeConnectStatusResponse(bool IsConnected, bool ChargesEnabled, bool PayoutsEnabled);
