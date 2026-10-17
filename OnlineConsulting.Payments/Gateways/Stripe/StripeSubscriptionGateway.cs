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
        }, ToRequestOptions(idempotencyKey, request.ConnectedAccountId), cancellationToken);

        return new SubscriptionCustomerResult(customer.Id);
    }

    public async Task<SubscriptionPriceResult> EnsurePriceAsync(EnsurePriceRequest request, CancellationToken cancellationToken = default)
    {
        var requestOptions = ToRequestOptions(null, request.ConnectedAccountId);

        var productService = new ProductService(_client);

        var product = await productService.CreateAsync(new ProductCreateOptions
        {
            Name = request.Name,
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, requestOptions, cancellationToken);

        var priceService = new PriceService(_client);

        var price = await priceService.CreateAsync(new PriceCreateOptions
        {
            Product = product.Id,
            UnitAmount = ToMinorUnits(request.Amount),
            Currency = request.Currency.ToLowerInvariant(),
            Recurring = new PriceRecurringOptions { Interval = MapInterval(request.BillingCycle) },
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, requestOptions, cancellationToken);

        return new SubscriptionPriceResult(product.Id, price.Id);
    }

    public async Task<bool> PriceExistsAsync(string providerPriceId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new PriceService(_client);

        try
        {
            var price = await service.GetAsync(providerPriceId, requestOptions: ToRequestOptions(null, connectedAccountId), cancellationToken: cancellationToken);
            return price.Active;
        }
        catch (StripeException exception) when (exception.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default)
    {
        var requestOptions = ToRequestOptions(null, request.ConnectedAccountId);

        var paymentMethodService = new PaymentMethodService(_client);

        var attachedPaymentMethod = await paymentMethodService.AttachAsync(request.PaymentMethodId, new PaymentMethodAttachOptions
        {
            Customer = request.ProviderCustomerId,
        }, requestOptions, cancellationToken);

        var customerService = new CustomerService(_client);

        _ = await customerService.UpdateAsync(request.ProviderCustomerId, new CustomerUpdateOptions
        {
            InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = attachedPaymentMethod.Id },
        }, requestOptions, cancellationToken);

        List<SubscriptionDiscountOptions>? discounts = null;

        if (request.DiscountAmount is > 0)
        {
            var couponService = new CouponService(_client);
            var coupon = await couponService.CreateAsync(new CouponCreateOptions
            {
                AmountOff = ToMinorUnits(request.DiscountAmount.Value),
                Currency = "usd",
                Duration = "once",
            }, requestOptions, cancellationToken);

            discounts = [new SubscriptionDiscountOptions { Coupon = coupon.Id }];
        }

        var subscriptionService = new SubscriptionService(_client);

        var subscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = request.ProviderCustomerId,
            Items = [new SubscriptionItemOptions { Price = request.ProviderPriceId },
                .. (request.AdditionalPriceIds ?? []).Select(priceId => new SubscriptionItemOptions { Price = priceId })],
            Discounts = discounts,
            TrialPeriodDays = request.TrialDays,
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, ToRequestOptions(idempotencyKey, request.ConnectedAccountId), cancellationToken);

        return ToSubscriptionResult(subscription);
    }

    public async Task<SubscriptionResult> CancelSubscriptionAsync(string providerSubscriptionId, bool atPeriodEnd = false, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        var requestOptions = ToRequestOptions(null, connectedAccountId);

        if (atPeriodEnd)
        {
            var updated = await service.UpdateAsync(providerSubscriptionId, new SubscriptionUpdateOptions { CancelAtPeriodEnd = true }, requestOptions, cancellationToken);

            return ToSubscriptionResult(updated);
        }

        var subscription = await service.CancelAsync(providerSubscriptionId, requestOptions: requestOptions, cancellationToken: cancellationToken);

        return ToSubscriptionResult(subscription);
    }

    public async Task<SubscriptionResult> ReactivateSubscriptionAsync(string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);

        var updated = await service.UpdateAsync(providerSubscriptionId, new SubscriptionUpdateOptions { CancelAtPeriodEnd = false }, ToRequestOptions(null, connectedAccountId), cancellationToken);

        return ToSubscriptionResult(updated);
    }

    public async Task<SubscriptionResult> UpdateSubscriptionPriceAsync(string providerSubscriptionId, string newProviderPriceId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        var requestOptions = ToRequestOptions(null, connectedAccountId);

        var subscription = await service.GetAsync(providerSubscriptionId, requestOptions: requestOptions, cancellationToken: cancellationToken);

        var itemId = subscription.Items.Data[0].Id;

        var updated = await service.UpdateAsync(providerSubscriptionId, new SubscriptionUpdateOptions
        {
            Items = [new SubscriptionItemOptions { Id = itemId, Price = newProviderPriceId }],
            ProrationBehavior = "create_prorations",
        }, requestOptions, cancellationToken);

        return ToSubscriptionResult(updated);
    }

    public async Task<SubscriptionResult> PauseSubscriptionAsync(string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);

        var updated = await service.UpdateAsync(providerSubscriptionId, new SubscriptionUpdateOptions
        {
            PauseCollection = new SubscriptionPauseCollectionOptions { Behavior = "void" },
        }, ToRequestOptions(null, connectedAccountId), cancellationToken);

        return ToSubscriptionResult(updated);
    }

    /// <summary>Stripe.net has no property to clear pause_collection; ExtraParams sends the literal empty object the API requires to unset it.</summary>
    public async Task<SubscriptionResult> ResumeSubscriptionAsync(string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        var options = new SubscriptionUpdateOptions();
        options.AddExtraParam("pause_collection", "");

        var updated = await service.UpdateAsync(providerSubscriptionId, options, ToRequestOptions(null, connectedAccountId), cancellationToken);

        return ToSubscriptionResult(updated);
    }

    public async Task CancelAndRefundAsync(string providerCustomerId, string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var requestOptions = ToRequestOptions(null, connectedAccountId);

        var subscriptionService = new SubscriptionService(_client);
        var subscription = await subscriptionService.GetAsync(providerSubscriptionId, requestOptions: requestOptions, cancellationToken: cancellationToken);
        if (subscription.Status is not ("canceled" or "incomplete_expired"))
        {
            _ = await subscriptionService.CancelAsync(providerSubscriptionId, requestOptions: requestOptions, cancellationToken: cancellationToken);
        }

        var chargeService = new ChargeService(_client);
        var refundService = new RefundService(_client);
        var charges = await chargeService.ListAsync(new ChargeListOptions { Customer = providerCustomerId, Limit = 100 }, requestOptions, cancellationToken);
        foreach (var charge in charges.Data.Where(c => c.Paid && !c.Refunded && c.AmountRefunded < c.Amount))
        {
            _ = await refundService.CreateAsync(new RefundCreateOptions { Charge = charge.Id },
                new RequestOptions { IdempotencyKey = $"signup-rollback-refund:{charge.Id}", StripeAccount = connectedAccountId }, cancellationToken);
        }
    }

    public async Task<SubscriptionInvoice?> GetLatestInvoiceAsync(string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        var subscription = await service.GetAsync(providerSubscriptionId, new SubscriptionGetOptions { Expand = ["latest_invoice"] }, ToRequestOptions(null, connectedAccountId), cancellationToken);
        return subscription.LatestInvoice is { } invoice ? ToSubscriptionInvoice(invoice) : null;
    }

    private static SubscriptionInvoice ToSubscriptionInvoice(Invoice invoice) => new(
        invoice.Id,
        invoice.Number,
        invoice.AmountPaid / 100m,
        invoice.Currency.ToUpperInvariant(),
        invoice.Status,
        invoice.HostedInvoiceUrl,
        invoice.InvoicePdf,
        invoice.Lines.Data.Count > 0 ? new DateTimeOffset(invoice.Lines.Data[0].Period.Start, TimeSpan.Zero) : null,
        invoice.Lines.Data.Count > 0 ? new DateTimeOffset(invoice.Lines.Data.Max(l => l.Period.End), TimeSpan.Zero) : null,
        invoice.BillingReason,
        [.. invoice.Lines.Data.Select(l => new SubscriptionInvoiceLine(l.Description ?? "Subscription", l.Amount / 100m))]);

    public async Task<string> AddSubscriptionItemAsync(string providerSubscriptionId, string providerPriceId, string? idempotencyKey = null, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionItemService(_client);

        var item = await service.CreateAsync(new SubscriptionItemCreateOptions
        {
            Subscription = providerSubscriptionId,
            Price = providerPriceId,
        }, ToRequestOptions(idempotencyKey, connectedAccountId), cancellationToken);

        return item.Id;
    }

    public async Task RemoveSubscriptionItemAsync(string providerSubscriptionItemId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionItemService(_client);

        _ = await service.DeleteAsync(providerSubscriptionItemId, requestOptions: ToRequestOptions(null, connectedAccountId), cancellationToken: cancellationToken);
    }

    /// <summary>stripeEvent.Account - see StripePaymentGateway.VerifyAndParseWebhookAsync.</summary>
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
                        : Task.FromResult<SubscriptionWebhookEvent?>(new SubscriptionWebhookEvent(subscription.Id, referenceId, SubscriptionEventKinds.Cancelled, ConnectedAccountId: stripeEvent.Account));
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

                    return Task.FromResult<SubscriptionWebhookEvent?>(new SubscriptionWebhookEvent(subscriptionDetails.SubscriptionId, referenceId, eventKind, newRenewalDate,
                        eventKind == SubscriptionEventKinds.Renewed ? ToSubscriptionInvoice(invoice) : null, stripeEvent.Account));
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

        var itemIdsByPriceId = subscription.Items.Data.ToDictionary(item => item.Price.Id, item => item.Id);

        return new SubscriptionResult(subscription.Id, MapStatus(subscription.Status), currentPeriodEnd, ItemIdsByPriceId: itemIdsByPriceId);
    }

    private static RequestOptions? ToRequestOptions(string? idempotencyKey, string? connectedAccountId) =>
        idempotencyKey is null && connectedAccountId is null ? null : new RequestOptions { IdempotencyKey = idempotencyKey, StripeAccount = connectedAccountId };

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
