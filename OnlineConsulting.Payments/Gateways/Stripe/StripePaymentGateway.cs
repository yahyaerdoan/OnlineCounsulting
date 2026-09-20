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
        }, new RequestOptions { IdempotencyKey = request.IdempotencyKey }, cancellationToken);

        return new PaymentIntentResult(intent.Id, MapStatus(intent.Status), intent.ClientSecret);
    }

    public async Task<PaymentStatusResult> GetStatusAsync(string providerPaymentId, CancellationToken cancellationToken = default)
    {
        var service = new PaymentIntentService(_client);
        var intent = await service.GetAsync(providerPaymentId, cancellationToken: cancellationToken);
        return new PaymentStatusResult(intent.Id, MapStatus(intent.Status), intent.ClientSecret);
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
        // Reject before EventUtility - it assumes a non-null header and throws NullReferenceException, not StripeException, otherwise.
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(rawBody))
        {
            return Task.FromResult<PaymentWebhookEvent?>(null);
        }

        // Caught broadly on purpose - untrusted external input, any construction failure means "not a valid webhook call", never a 500.
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
