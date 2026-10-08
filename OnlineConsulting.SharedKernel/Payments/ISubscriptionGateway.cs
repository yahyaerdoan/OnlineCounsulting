namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>One implementation per provider that supports recurring billing (currently Stripe only - PayPal/Mock's IPaymentGateway stays untouched). Callers depend on this interface only, never on a concrete gateway type.</summary>
public interface ISubscriptionGateway
{
    /// <summary>Matches one of PaymentProviderNames - also the keyed-DI service key this implementation is registered under.</summary>
    string ProviderName { get; }

    /// <summary>False when the provider has no concept of extra priced line items (PayPal); callers should check this before AddSubscriptionItemAsync/RemoveSubscriptionItemAsync rather than let NotSupportedException surface.</summary>
    bool SupportsMultipleItems { get; }

    /// <summary>Gets or creates the provider-side customer; always creates when no id is passed in. idempotencyKey, if given, stops a retried call from creating a duplicate customer.</summary>
    Task<SubscriptionCustomerResult> EnsureCustomerAsync(EnsureCustomerRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default);

    /// <summary>Creates the provider-side product/price for a MembershipPlan. Prices are immutable on the provider side, so this is called once at plan creation - not on every update.</summary>
    Task<SubscriptionPriceResult> EnsurePriceAsync(EnsurePriceRequest request, CancellationToken cancellationToken = default);

    /// <summary>Attaches the given payment method to the customer as default, then starts a recurring subscription against the given price. idempotencyKey - see EnsureCustomerAsync.</summary>
    Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default);

    /// <summary>atPeriodEnd=true keeps billing until period end (PayPal ignores this and cancels immediately regardless).</summary>
    Task<SubscriptionResult> CancelSubscriptionAsync(string providerSubscriptionId, bool atPeriodEnd = false, CancellationToken cancellationToken = default);

    /// <summary>Undoes a CancelSubscriptionAsync(atPeriodEnd: true) before the period ends, so the subscription renews again. Not supported
    /// by providers that cancel immediately (PayPal).</summary>
    Task<SubscriptionResult> ReactivateSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default);

    /// <summary>Plan upgrade/downgrade: swaps the subscription's price, prorating for the remainder of the period. Not supported by every provider (PayPal).</summary>
    Task<SubscriptionResult> UpdateSubscriptionPriceAsync(string providerSubscriptionId, string newProviderPriceId, CancellationToken cancellationToken = default);

    /// <summary>Stops billing indefinitely without cancelling; unlike most other capability-gated methods here, PayPal supports this natively.</summary>
    Task<SubscriptionResult> PauseSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default);

    /// <summary>Reverses PauseSubscriptionAsync - billing resumes normally.</summary>
    Task<SubscriptionResult> ResumeSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default);

    /// <summary>Adds a priced line item to an existing subscription, prorated; returns the new item id, which callers must persist to remove it later. Not supported by every provider (PayPal).</summary>
    Task<string> AddSubscriptionItemAsync(string providerSubscriptionId, string providerPriceId, string? idempotencyKey = null, CancellationToken cancellationToken = default);

    /// <summary>Removes one line item, prorated refund/credit for the remainder of the period. Not supported by every provider (PayPal).</summary>
    Task RemoveSubscriptionItemAsync(string providerSubscriptionItemId, CancellationToken cancellationToken = default);

    /// <summary>Undoes a signup that couldn't finish after it was paid: cancels the subscription now and refunds what the customer paid on it.
    /// The customer must be one created for this signup only, since every charge on it is refunded. Safe to call again (already
    /// cancelled or refunded is fine).</summary>
    Task CancelAndRefundAsync(string providerCustomerId, string providerSubscriptionId, CancellationToken cancellationToken = default);

    /// <summary>The subscription's most recent invoice as the provider issued it (number, amount, links); null when the provider has none to show (Mock, PayPal).</summary>
    Task<SubscriptionInvoice?> GetLatestInvoiceAsync(string providerSubscriptionId, CancellationToken cancellationToken = default);

    /// <summary>Verifies the webhook signature and normalizes the payload; null if it's not a subscription-lifecycle event.</summary>
    Task<SubscriptionWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default);
}
