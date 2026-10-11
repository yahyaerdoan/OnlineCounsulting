using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Payments;
using Stripe;

namespace OnlineConsulting.Payments.Gateways.Stripe;

public class StripePaymentGateway(IOptions<PaymentOptions> options) : IPaymentGateway
{
    private readonly StripeClient _client = new(options.Value.Stripe.SecretKey);
    private readonly string _webhookSecret = options.Value.Stripe.WebhookSecret;

    public string ProviderName => PaymentProviderNames.Stripe;

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default)
    {
        var service = new PaymentIntentService(_client);

        var intent = await service.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = ToMinorUnits(request.Amount),
            Currency = request.Currency.ToLowerInvariant(),
            ReceiptEmail = request.CustomerEmail,
            Metadata = new Dictionary<string, string> { ["ReferenceId"] = request.ReferenceId },
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
        }, ToRequestOptions(request.IdempotencyKey, request.ConnectedAccountId), cancellationToken);

        return new PaymentIntentResult(intent.Id, MapStatus(intent.Status), intent.ClientSecret);
    }

    public async Task<PaymentStatusResult> GetStatusAsync(string providerPaymentId, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new PaymentIntentService(_client);
        var intent = await service.GetAsync(providerPaymentId, requestOptions: ToRequestOptions(null, connectedAccountId), cancellationToken: cancellationToken);

        return new PaymentStatusResult(intent.Id, MapStatus(intent.Status), intent.ClientSecret);
    }

    public async Task<PaymentStatusResult> RefundAsync(string providerPaymentId, decimal? amount = null, string? connectedAccountId = null, CancellationToken cancellationToken = default)
    {
        var service = new RefundService(_client);

        var refund = await service.CreateAsync(new RefundCreateOptions
        {
            PaymentIntent = providerPaymentId,
            Amount = amount.HasValue ? ToMinorUnits(amount.Value) : null,
        }, ToRequestOptions(null, connectedAccountId), cancellationToken);

        return new PaymentStatusResult(providerPaymentId, refund.Status == "succeeded" ? PaymentStatuses.Refunded : PaymentStatuses.Pending);
    }

    /// <summary>Verifies the Stripe signature and parses a payment-intent webhook event; returns null for anything invalid or irrelevant rather than throwing, since input is untrusted external data.
    /// stripeEvent.Account is set for events from a tenant's Connect account (the platform's Connect webhook endpoint receives these alongside its own).</summary>
    public Task<PaymentWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(rawBody))
        {
            return Task.FromResult<PaymentWebhookEvent?>(null);
        }

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(rawBody, signatureHeader, _webhookSecret);
        }
        catch (Exception)
        {
            return Task.FromResult<PaymentWebhookEvent?>(null);
        }

        if (stripeEvent.Data.Object is not PaymentIntent intent)
        {
            return Task.FromResult<PaymentWebhookEvent?>(null);
        }

        var referenceId = intent.Metadata.GetValueOrDefault("ReferenceId");

        return referenceId is null
            ? Task.FromResult<PaymentWebhookEvent?>(null)
            : stripeEvent.Type switch
            {
                "payment_intent.succeeded" => Task.FromResult<PaymentWebhookEvent?>(new PaymentWebhookEvent(intent.Id, referenceId, Succeeded: true, stripeEvent.Account)),
                "payment_intent.payment_failed" => Task.FromResult<PaymentWebhookEvent?>(new PaymentWebhookEvent(intent.Id, referenceId, Succeeded: false, stripeEvent.Account)),
                _ => Task.FromResult<PaymentWebhookEvent?>(null),
            };
    }

    private static RequestOptions? ToRequestOptions(string? idempotencyKey, string? connectedAccountId) =>
        idempotencyKey is null && connectedAccountId is null ? null : new RequestOptions { IdempotencyKey = idempotencyKey, StripeAccount = connectedAccountId };

    private static long ToMinorUnits(decimal amount) => (long)(amount * 100);

    private static string MapStatus(string stripeStatus) => stripeStatus switch
    {
        "succeeded" => PaymentStatuses.Succeeded,
        "processing" => PaymentStatuses.Processing,
        "canceled" => PaymentStatuses.Failed,
        _ => PaymentStatuses.Pending,
    };
}
