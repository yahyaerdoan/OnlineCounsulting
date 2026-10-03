namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Contracts;

/// <summary>ClientSecret mirrors SubscribeToMembershipResult - null for Stripe, a PayPal approval URL when PayPal is active. Null on a resume call where the base subscription was already created in a prior attempt.</summary>
public record ActivateTenantSubscriptionResult(Guid TenantId, string? ClientSecret);
