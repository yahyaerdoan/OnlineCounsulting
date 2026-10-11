namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

/// <summary>Public - just enough for Stripe.js to run in the right account context (stripeAccount option). Not sensitive, this id is visible in Stripe's own redirect URLs.</summary>
public record TenantPaymentAccountResponse(string? ConnectedAccountId);
