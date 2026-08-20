using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Payments;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OnlineConsulting.Payments.Gateways.PayPal;

/// <summary>Structurally complete PayPal Orders v2 implementation, proving IPaymentGateway isn't shaped around Stripe specifically - but untested end-to-end (no PayPal sandbox credentials configured in this environment). Uses raw REST calls instead of PayPal's SDK to keep the dependency light for a provider that isn't active yet. Takes IHttpClientFactory (not a typed HttpClient) because this gateway is registered as a singleton - a constructor-captured HttpClient would pin one handler/connection pool for the app's whole lifetime instead of letting IHttpClientFactory rotate handlers.</summary>
public class PayPalPaymentGateway(IHttpClientFactory httpClientFactory, IOptions<PaymentOptions> options) : IPaymentGateway
{
    private readonly PayPalOptions _options = options.Value.PayPal;

    public string ProviderName => PaymentProviderNames.PayPal;

    private HttpClient CreateClient() => httpClientFactory.CreateClient(nameof(PayPalPaymentGateway));

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v2/checkout/orders");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpRequest.Headers.Add("PayPal-Request-Id", request.IdempotencyKey);
        httpRequest.Content = JsonContent.Create(new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    reference_id = request.ReferenceId,
                    amount = new { currency_code = request.Currency.ToUpperInvariant(), value = request.Amount.ToString("F2") },
                },
            },
        });

        using var client = CreateClient();
        using var response = await client.SendAsync(httpRequest, cancellationToken);
        _ = response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<PayPalOrder>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("PayPal returned an empty order response.");

        // No client-secret concept in PayPal - the "approve" link is what a frontend redirects the payer to instead.
        var approveLink = order.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href;

        return new PaymentIntentResult(order.Id, MapStatus(order.Status), approveLink);
    }

    public async Task<PaymentStatusResult> GetStatusAsync(string providerPaymentId, CancellationToken cancellationToken = default)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/v2/checkout/orders/{providerPaymentId}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var client = CreateClient();
        using var response = await client.SendAsync(httpRequest, cancellationToken);
        _ = response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<PayPalOrder>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("PayPal returned an empty order response.");

        return new PaymentStatusResult(order.Id, MapStatus(order.Status));
    }

    /// <summary>Simplified: assumes providerPaymentId is an order that was fully captured under a single capture. A production version would need the capture id (returned when the order is captured), not the order id, to refund correctly.</summary>
    public async Task<PaymentStatusResult> RefundAsync(string providerPaymentId, decimal? amount = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/v2/payments/captures/{providerPaymentId}/refund");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        if (amount.HasValue)
        {
            httpRequest.Content = JsonContent.Create(new { amount = new { currency_code = "USD", value = amount.Value.ToString("F2") } });
        }

        using var client = CreateClient();
        using var response = await client.SendAsync(httpRequest, cancellationToken);
        _ = response.EnsureSuccessStatusCode();

        return new PaymentStatusResult(providerPaymentId, PaymentStatuses.Refunded);
    }

    public async Task<PaymentWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default)
    {
        // PayPal verifies webhooks via a callback REST call (not a local HMAC check like Stripe) - it needs the
        // original headers, which this interface doesn't carry through. Left unimplemented since PayPal isn't
        // the active provider; wiring this properly means threading the raw HttpRequest headers down to here.
        await Task.CompletedTask;
        throw new NotSupportedException("PayPal webhook verification needs the original request headers - not implemented until PayPal is actually activated.");
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using var client = CreateClient();
        return await PayPalAuth.GetAccessTokenAsync(client, _options, cancellationToken);
    }

    private static string MapStatus(string paypalStatus) => paypalStatus switch
    {
        "COMPLETED" => PaymentStatuses.Succeeded,
        "VOIDED" => PaymentStatuses.Failed,
        _ => PaymentStatuses.Pending,
    };

    private record PayPalOrder(string Id, string Status, List<PayPalLink>? Links);
}
