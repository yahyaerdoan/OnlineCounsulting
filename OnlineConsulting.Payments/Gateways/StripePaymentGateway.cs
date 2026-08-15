using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Payments;
using Stripe;

namespace OnlineConsulting.Payments.Gateways;

public class StripePaymentGateway : IPaymentGateway
{
    private readonly StripeClient _client;
    private readonly string _webhookSecret;

    public StripePaymentGateway(IOptions<PaymentOptions> options)
    {
        _client = new StripeClient(options.Value.Stripe.SecretKey);
        _webhookSecret = options.Value.Stripe.WebhookSecret;
    }

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
        }, new RequestOptions { IdempotencyKey = request.IdempotencyKey }, cancellationToken);

        return new PaymentIntentResult(intent.Id, MapStatus(intent.Status), intent.ClientSecret);
    }

    public async Task<PaymentStatusResult> GetStatusAsync(string providerPaymentId, CancellationToken cancellationToken = default)
    {
        var service = new PaymentIntentService(_client);
        var intent = await service.GetAsync(providerPaymentId, cancellationToken: cancellationToken);
        return new PaymentStatusResult(intent.Id, MapStatus(intent.Status));
    }

    public async Task<PaymentStatusResult> RefundAsync(string providerPaymentId, decimal? amount = null, CancellationToken cancellationToken = default)
    {
        var service = new RefundService(_client);
        var refund = await service.CreateAsync(new RefundCreateOptions
        {
            PaymentIntent = providerPaymentId,
            Amount = amount.HasValue ? ToMinorUnits(amount.Value) : null,
        }, cancellationToken: cancellationToken);

        return new PaymentStatusResult(providerPaymentId, refund.Status == "succeeded" ? PaymentStatuses.Refunded : PaymentStatuses.Pending);
    }

    public Task<PaymentWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default)
    {
        // A missing signature header (e.g. someone probing the endpoint directly, like via Swagger's
        // "Try it out") isn't a forged event - it's not a Stripe call at all - so reject it before ever
        // calling into EventUtility, which assumes a non-null header and throws NullReferenceException
        // rather than StripeException when it isn't.
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(rawBody))
            return Task.FromResult<PaymentWebhookEvent?>(null);

        // EventUtility.ConstructEvent throws StripeException on a bad/malformed signature - that's the whole
        // point of verification (rejects forged webhook calls). It can also throw NullReferenceException
        // from inside the SDK itself for a structurally-off payload (e.g. a missing "api_version" field) -
        // caught broadly here on purpose: this endpoint receives untrusted external input, so any failure to
        // construct/validate the event must mean "not a valid webhook call", never a 500.
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
            return Task.FromResult<PaymentWebhookEvent?>(null);

        var referenceId = intent.Metadata.GetValueOrDefault("ReferenceId");
        if (referenceId is null)
            return Task.FromResult<PaymentWebhookEvent?>(null);

        return stripeEvent.Type switch
        {
            "payment_intent.succeeded" => Task.FromResult<PaymentWebhookEvent?>(new PaymentWebhookEvent(intent.Id, referenceId, Succeeded: true)),
            "payment_intent.payment_failed" => Task.FromResult<PaymentWebhookEvent?>(new PaymentWebhookEvent(intent.Id, referenceId, Succeeded: false)),
            _ => Task.FromResult<PaymentWebhookEvent?>(null),
        };
    }

    private static long ToMinorUnits(decimal amount) => (long)(amount * 100);

    private static string MapStatus(string stripeStatus) => stripeStatus switch
    {
        "succeeded" => PaymentStatuses.Succeeded,
        "canceled" => PaymentStatuses.Failed,
        _ => PaymentStatuses.Pending,
    };
}
