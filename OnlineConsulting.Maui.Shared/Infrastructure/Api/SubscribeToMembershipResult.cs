namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/memberships/subscribe's response shape. ClientSecret is null for Stripe/Mock
/// (already settled server-side) and a PayPal approval URL the client must redirect to when PayPal is active.</summary>
public record SubscribeToMembershipResult(Guid CustomerMembershipId, string? ClientSecret, decimal? AppliedCreditAmount, decimal? AppliedPromoDiscountAmount = null);
