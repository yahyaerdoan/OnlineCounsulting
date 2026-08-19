namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>One implementation per provider that supports recurring billing (currently Stripe only - PayPal/Mock's IPaymentGateway stays untouched). Callers depend on this interface only, never on a concrete gateway type.</summary>
public interface ISubscriptionGateway
{
    /// <summary>Matches one of PaymentProviderNames - also the keyed-DI service key this implementation is registered under.</summary>
    string ProviderName { get; }

    /// <summary>False for providers whose subscription model has no concept of independently-priced additional line items (PayPal - see PayPalSubscriptionGateway.AddSubscriptionItemAsync). Callers that would otherwise reach AddSubscriptionItemAsync/RemoveSubscriptionItemAsync with more than one module involved should check this first and reject cleanly instead of letting the NotSupportedException surface.</summary>
    bool SupportsMultipleItems { get; }

    /// <summary>Gets or creates the provider-side customer for a user. Idempotency is the caller's responsibility (CustomerMembership.ProviderCustomerId is persisted once created), so this always creates when no id is passed in from the caller. idempotencyKey, when given, is forwarded to the provider so a retried call with the same key never creates a second customer (see StripeSubscriptionGateway) - optional and unused by callers that already dedup some other way (e.g. SubscribeToMembershipCommand).</summary>
    Task<SubscriptionCustomerResult> EnsureCustomerAsync(EnsureCustomerRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default);

    /// <summary>Creates the provider-side product/price for a MembershipPlan. Prices are immutable on the provider side, so this is called once at plan creation - not on every update.</summary>
    Task<SubscriptionPriceResult> EnsurePriceAsync(EnsurePriceRequest request, CancellationToken cancellationToken = default);

    /// <summary>Attaches the given payment method to the customer as default, then starts a recurring subscription against the given price. idempotencyKey - see EnsureCustomerAsync.</summary>
    Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default);

    /// <summary>Cancels immediately (no cancel-at-period-end support in this phase).</summary>
    Task<SubscriptionResult> CancelSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default);

    /// <summary>Adds one more line item (its own price) to an already-created subscription - billed immediately, prorated for the remainder of the current period. Returns the new provider-side subscription item id, which callers must persist to later remove that specific line. Not supported by every provider - see PayPalSubscriptionGateway. idempotencyKey - see EnsureCustomerAsync.</summary>
    Task<string> AddSubscriptionItemAsync(string providerSubscriptionId, string providerPriceId, string? idempotencyKey = null, CancellationToken cancellationToken = default);

    /// <summary>Removes one line item from a subscription - prorated refund/credit for the remainder of the current period, same as AddSubscriptionItemAsync's proration direction. Not supported by every provider - see PayPalSubscriptionGateway.</summary>
    Task RemoveSubscriptionItemAsync(string providerSubscriptionItemId, CancellationToken cancellationToken = default);

    /// <summary>Verifies the provider's webhook signature and normalizes the payload. Returns null if the payload isn't a subscription-lifecycle event this gateway cares about.</summary>
    Task<SubscriptionWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default);
}

public record EnsureCustomerRequest(string ReferenceId, string Email);

public record SubscriptionCustomerResult(string ProviderCustomerId);

public record EnsurePriceRequest(string ReferenceId, string Name, decimal Amount, string Currency, string BillingCycle);

public record SubscriptionPriceResult(string ProviderProductId, string ProviderPriceId);

/// <summary>DiscountAmount is a one-time, first-invoice-only discount (e.g. account credit applied at subscribe time) - not a recurring price change. Ignored by providers with no such concept in this phase (PayPal - see PayPalSubscriptionGateway).</summary>
public record CreateSubscriptionRequest(string ProviderCustomerId, string ProviderPriceId, string PaymentMethodId, string ReferenceId, decimal? DiscountAmount = null);

/// <summary>ClientSecret mirrors PaymentIntentResult.ClientSecret's reuse across providers: null for Stripe (the payment method is already attached server-side, nothing left for the client to confirm), the subscriber's PayPal approval URL for PayPal (the payer must be redirected there before the subscription activates - PayPal has no server-side "attach card with a secret key alone" flow). FirstItemProviderId is the provider-side subscription item id for the single line item CreateSubscriptionAsync creates - null for providers with no such concept (PayPal), lets multi-item callers (Tenancy) capture it without a redundant follow-up call.</summary>
public record SubscriptionResult(string ProviderSubscriptionId, string Status, DateTimeOffset CurrentPeriodEnd, string? ClientSecret = null, string? FirstItemProviderId = null);

/// <summary>ReferenceId round-trips whatever CreateSubscriptionRequest.ReferenceId was, so the webhook handler can map back to the CustomerMembership without querying the provider for it. NewRenewalDate is only set when EventKind is Renewed.</summary>
public record SubscriptionWebhookEvent(string ProviderSubscriptionId, string ReferenceId, string EventKind, DateTimeOffset? NewRenewalDate = null);

/// <summary>SubscriptionWebhookEvent.EventKind values.</summary>
public static class SubscriptionEventKinds
{
    public const string Renewed = "Renewed";
    public const string Cancelled = "Cancelled";
    public const string PaymentFailed = "PaymentFailed";
}
