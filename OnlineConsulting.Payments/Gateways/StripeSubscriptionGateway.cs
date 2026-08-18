using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Payments;
using Stripe;

namespace OnlineConsulting.Payments.Gateways;

public class StripeSubscriptionGateway : ISubscriptionGateway
{
    private readonly StripeClient _client;
    private readonly string _webhookSecret;

    public StripeSubscriptionGateway(IOptions<PaymentOptions> options)
    {
        _client = new StripeClient(options.Value.Stripe.SecretKey);
        _webhookSecret = options.Value.Stripe.WebhookSecret;
    }

    public string ProviderName => PaymentProviderNames.Stripe;

    public async Task<SubscriptionCustomerResult> EnsureCustomerAsync(EnsureCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var service = new CustomerService(_client);
        var customer = await service.CreateAsync(new CustomerCreateOptions
        {
            Email = request.Email,
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, cancellationToken: cancellationToken);

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

    public async Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        var paymentMethodService = new PaymentMethodService(_client);
        var attachedPaymentMethod = await paymentMethodService.AttachAsync(request.PaymentMethodId, new PaymentMethodAttachOptions
        {
            Customer = request.ProviderCustomerId,
        }, cancellationToken: cancellationToken);

        var customerService = new CustomerService(_client);
        await customerService.UpdateAsync(request.ProviderCustomerId, new CustomerUpdateOptions
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
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
        }, cancellationToken: cancellationToken);

        return ToSubscriptionResult(subscription);
    }

    public async Task<SubscriptionResult> CancelSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default)
    {
        var service = new SubscriptionService(_client);
        var subscription = await service.CancelAsync(providerSubscriptionId, cancellationToken: cancellationToken);
        return ToSubscriptionResult(subscription);
    }

    public Task<SubscriptionWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(rawBody))
            return Task.FromResult<SubscriptionWebhookEvent?>(null);

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
                    return Task.FromResult<SubscriptionWebhookEvent?>(null);

                var referenceId = subscription.Metadata.GetValueOrDefault("ReferenceId");
                return referenceId is null
                    ? Task.FromResult<SubscriptionWebhookEvent?>(null)
                    : Task.FromResult<SubscriptionWebhookEvent?>(new SubscriptionWebhookEvent(subscription.Id, referenceId, SubscriptionEventKinds.Cancelled));
            }
            case "invoice.payment_failed":
            case "invoice.paid":
            {
                if (stripeEvent.Data.Object is not Invoice invoice)
                    return Task.FromResult<SubscriptionWebhookEvent?>(null);

                var subscriptionDetails = invoice.Parent?.SubscriptionDetails;
                var referenceId = subscriptionDetails?.Metadata?.GetValueOrDefault("ReferenceId");
                if (subscriptionDetails?.SubscriptionId is null || referenceId is null)
                    return Task.FromResult<SubscriptionWebhookEvent?>(null);

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

        return new SubscriptionResult(subscription.Id, MapStatus(subscription.Status), currentPeriodEnd);
    }

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
