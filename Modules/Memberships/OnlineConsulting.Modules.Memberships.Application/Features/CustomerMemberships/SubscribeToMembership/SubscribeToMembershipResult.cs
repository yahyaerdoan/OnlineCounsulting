namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.SubscribeToMembership;

/// <summary>ClientSecret is null for Stripe (already confirmed server-side) or a PayPal approval URL to redirect to; AppliedCreditAmount is the clamped amount actually sent to the gateway, which the caller must spend exactly.</summary>
public record SubscribeToMembershipResult(Guid CustomerMembershipId, string? ClientSecret, decimal? AppliedCreditAmount, decimal? AppliedPromoDiscountAmount = null);
