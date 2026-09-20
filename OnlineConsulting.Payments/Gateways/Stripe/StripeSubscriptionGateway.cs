using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Payments;
using Stripe;

namespace OnlineConsulting.Payments.Gateways.Stripe;

public class StripeSubscriptionGateway(IOptions<PaymentOptions> options) : ISubscriptionGateway
{
    private readonly StripeClient _client = new(options.Value.Stripe.SecretKey);
    private readonly string _webhookSecret = options.Value.Stripe.WebhookSecret;

    public string ProviderName => PaymentProviderNames.Stripe;

    public bool SupportsMultipleItems => true;

    public async Task<SubscriptionCustomerResult> EnsureCustomerAsync(EnsureCustomerRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default)
    {
        var service = new CustomerService(_client);
        var customer = await service.CreateAsync(new CustomerCreateOptions
        {
            Email = request.Email,
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, ToRequestOptions(idempotencyKey), cancellationToken);

        return new SubscriptionCustomerResult(customer.Id);
    }

    public async Task<SubscriptionPriceResult> EnsurePriceAsync(EnsurePriceRequest request, CancellationToken cancellationToken = default)
    {
        var productService = new ProductService(_client);
        var product = await productService.CreateAsync(new ProductCreateOptions
        {
            Name = request.Name,
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, cancellationToken: cancellationToken);

        var priceService = new PriceService(_client);
        var price = await priceService.CreateAsync(new PriceCreateOptions
        {
            Product = product.Id,
            UnitAmount = ToMinorUnits(request.Amount),
            Currency = request.Currency.ToLowerInvariant(),
            Recurring = new PriceRecurringOptions { Interval = MapInterval(request.BillingCycle) },
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, cancellationToken: cancellationToken);

        return new SubscriptionPriceResult(product.Id, price.Id);
    }

    public async Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default)
    {
        var paymentMethodService = new PaymentMethodService(_client);
        var attachedPaymentMethod = await paymentMethodService.AttachAsync(request.PaymentMethodId, new PaymentMethodAttachOptions
        {
            Customer = request.ProviderCustomerId,
        }, cancellationToken: cancellationToken);

        var customerService = new CustomerService(_client);
        _ = await customerService.UpdateAsync(request.ProviderCustomerId, new CustomerUpdateOptions
        {
            InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = attachedPaymentMethod.Id },
        }, cancellationToken: cancellationToken);

        List<SubscriptionDiscountOptions>? discounts = null;
        if (request.DiscountAmount is > 0)
        {
            var couponService = new CouponService(_client);
            var coupon = await couponService.CreateAsync(new CouponCreateOptions
            {
                AmountOff = ToMinorUnits(request.DiscountAmount.Value),
                Currency = "usd",
                Duration = "once",
            }, cancellationToken: cancellationToken);

            discounts = [new SubscriptionDiscountOptions { Coupon = coupon.Id }];
        }

        var subscriptionService = new SubscriptionService(_client);
        var subscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = request.ProviderCustomerId,
            Items = [new SubscriptionItemOptions { Price = request.ProviderPriceId }],
            Discounts = discounts,
            TrialPeriodDays = request.TrialDays,
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, ToRequestOptions(idempotencyKey), cancellationToken);

        return ToSubscriptionResult(subscription);
    }

    public async Task<SubscriptionResult> CancelSubscriptionAsync(string providerSubscriptionId, bool atPeriodEnd = false, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        if (atPeriodEnd)
        {
            var updated = await service.UpdateAsync(providerSubscriptionId, new SubscriptionUpdateOptions { CancelAtPeriodEnd = true }, cancellationToken: cancellationToken);
            return ToSubscriptionResult(updated);
        }

        var subscription = await service.CancelAsync(providerSubscriptionId, cancellationToken: cancellationToken);
        return ToSubscriptionResult(subscription);
    }

    public async Task<SubscriptionResult> UpdateSubscriptionPriceAsync(string providerSubscriptionId, string newProviderPriceId, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        var subscription = await service.GetAsync(providerSubscriptionId, cancellationToken: cancellationToken);
        var itemId = subscription.Items.Data[0].Id;

        var updated = await service.UpdateAsync(providerSubscriptionId, new SubscriptionUpdateOptions
        {
            Items = [new SubscriptionItemOptions { Id = itemId, Price = newProviderPriceId }],
            ProrationBehavior = "create_prorations",
        }, cancellationToken: cancellationToken);

        return ToSubscriptionResult(updated);
    }

    public async Task<SubscriptionResult> PauseSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        var updated = await service.UpdateAsync(providerSubscriptionId, new SubscriptionUpdateOptions
        {
            PauseCollection = new SubscriptionPauseCollectionOptions { Behavior = "void" },
        }, cancellationToken: cancellationToken);

        return ToSubscriptionResult(updated);
    }

    /// <summary>Stripe.net has no property to explicitly clear pause_collection - omitting it leaves the
    /// existing value untouched, and the API only unsets it when the field is sent as an empty object.
    /// ExtraParams is the documented way to send that literal empty value.</summary>
    public async Task<SubscriptionResult> ResumeSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        var options = new SubscriptionUpdateOptions();
        options.AddExtraParam("pause_collection", "");
        var updated = await service.UpdateAsync(providerSubscriptionId, options, cancellationToken: cancellationToken);

        return ToSubscriptionResult(updated);
    }

    public async Task<string> AddSubscriptionItemAsync(string providerSubscriptionId, string providerPriceId, string? idempotencyKey = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionItemService(_client);
        var item = await service.CreateAsync(new SubscriptionItemCreateOptions
        {
            Subscription = providerSubscriptionId,
            Price = providerPriceId,
        }, ToRequestOptions(idempotencyKey), cancellationToken);

        return item.Id;
    }

    public async Task RemoveSubscriptionItemAsync(string providerSubscriptionItemId, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionItemService(_client);
        _ = await service.DeleteAsync(providerSubscriptionItemId, cancellationToken: cancellationToken);
    }

    public Task<SubscriptionWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(rawBody))
        {
            return Task.FromResult<SubscriptionWebhookEvent?>(null);
        }

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(rawBody, signatureHeader, _webhookSecret);
        }
        catch (Exception)
        {
            return Task.FromResult<SubscriptionWebhookEvent?>(null);
        }

        switch (stripeEvent.Type)
        {
            case "customer.subscription.deleted":
                {
                    if (stripeEvent.Data.Object is not Subscription subscription)
                    {
                        return Task.FromResult<SubscriptionWebhookEvent?>(null);
                    }

                    var referenceId = subscription.Metadata.GetValueOrDefault("ReferenceId");
                    return referenceId is null
                        ? Task.FromResult<SubscriptionWebhookEvent?>(null)
                        : Task.FromResult<SubscriptionWebhookEvent?>(new SubscriptionWebhookEvent(subscription.Id, referenceId, SubscriptionEventKinds.Cancelled));
                }
            case "invoice.payment_failed":
            case "invoice.paid":
                {
                    if (stripeEvent.Data.Object is not Invoice invoice)
                    {
                        return Task.FromResult<SubscriptionWebhookEvent?>(null);
                    }

                    var subscriptionDetails = invoice.Parent?.SubscriptionDetails;
                    var referenceId = subscriptionDetails?.Metadata?.GetValueOrDefault("ReferenceId");
                    if (subscriptionDetails?.SubscriptionId is null || referenceId is null)
                    {
                        return Task.FromResult<SubscriptionWebhookEvent?>(null);
                    }

                    var eventKind = stripeEvent.Type == "invoice.paid" ? SubscriptionEventKinds.Renewed : SubscriptionEventKinds.PaymentFailed;
                    var newRenewalDate = eventKind == SubscriptionEventKinds.Renewed && invoice.Lines.Data.Count > 0
                        ? new DateTimeOffset(invoice.Lines.Data[0].Period.End, TimeSpan.Zero)
                        : (DateTimeOffset?)null;

                    return Task.FromResult<SubscriptionWebhookEvent?>(new SubscriptionWebhookEvent(subscriptionDetails.SubscriptionId, referenceId, eventKind, newRenewalDate));
                }
            default:
                return Task.FromResult<SubscriptionWebhookEvent?>(null);
        }
    }

    private static SubscriptionResult ToSubscriptionResult(Subscription subscription)
    {
        var currentPeriodEnd = subscription.Items.Data.Count > 0
            ? new DateTimeOffset(subscription.Items.Data[0].CurrentPeriodEnd, TimeSpan.Zero)
            : DateTimeOffset.UtcNow;

        var firstItemProviderId = subscription.Items.Data.Count > 0 ? subscription.Items.Data[0].Id : null;

        return new SubscriptionResult(subscription.Id, MapStatus(subscription.Status), currentPeriodEnd, FirstItemProviderId: firstItemProviderId);
    }

    private static RequestOptions? ToRequestOptions(string? idempotencyKey) =>
        idempotencyKey is null ? null : new RequestOptions { IdempotencyKey = idempotencyKey };

    private static long ToMinorUnits(decimal amount) => (long)(amount * 100);

    private static string MapInterval(string billingCycle) => billingCycle switch
    {
        BillingCycles.Annual => "year",
        _ => "month",
    };

    private static string MapStatus(string stripeStatus) => stripeStatus switch
    {
        "active" or "trialing" => PaymentStatuses.Succeeded,
        "canceled" or "unpaid" => PaymentStatuses.Refunded,
        "past_due" or "incomplete" => PaymentStatuses.Pending,
        _ => PaymentStatuses.Failed,
    };
}
