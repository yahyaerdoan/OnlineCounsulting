namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;

/// <summary>Mirrors Commerce's CreateOrderResult - carries the provider's client-side completion token alongside the new membership id. Null for Stripe (already confirmed server-side); a PayPal approval URL the frontend must redirect the payer to when the active provider is PayPal. AppliedCreditAmount is the final amount actually sent to the gateway (request amount clamped to this plan's price) - the caller must spend exactly this much, not its own pre-clamp guess.</summary>
public record SubscribeToMembershipResult(Guid CustomerMembershipId, string? ClientSecret, decimal? AppliedCreditAmount);
