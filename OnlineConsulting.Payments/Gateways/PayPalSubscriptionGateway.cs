using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Payments;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Payments.Gateways;

/// <summary>Structurally complete PayPal Subscriptions v1 implementation, mirroring PayPalPaymentGateway - untested end-to-end (no PayPal sandbox credentials configured in this environment). PayPal has no server-side "attach a card with a secret key" flow like Stripe: the payer must be redirected to the "approve" link returned from CreateSubscriptionAsync before the subscription activates, so PaymentMethodId is ignored and the approval URL rides back on SubscriptionResult.ClientSecret instead (see that record's doc comment).</summary>
public class PayPalSubscriptionGateway(IHttpClientFactory httpClientFactory, IOptions<PaymentOptions> options) : ISubscriptionGateway
{
    private readonly PayPalOptions _options = options.Value.PayPal;

    public string ProviderName => PaymentProviderNames.PayPal;

    private HttpClient CreateClient() => httpClientFactory.CreateClient(nameof(PayPalSubscriptionGateway));

    /// <summary>No customer object exists in PayPal's Subscriptions API - the subscriber is specified directly (by email) on subscription creation - so this makes no API call, it just carries the email through as the "customer id" for CreateSubscriptionAsync to use.</summary>
    public Task<SubscriptionCustomerResult> EnsureCustomerAsync(EnsureCustomerRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionCustomerResult(request.Email));

    public async Task<SubscriptionPriceResult> EnsurePriceAsync(EnsurePriceRequest request, CancellationToken cancellationToken = default)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        using var productRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/catalogs/products");
        productRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        productRequest.Content = JsonContent.Create(new { name = request.Name, type = "SERVICE" });

        using var client = CreateClient();
        using var productResponse = await client.SendAsync(productRequest, cancellationToken);
        productResponse.EnsureSuccessStatusCode();
        var product = await productResponse.Content.ReadFromJsonAsync<PayPalProduct>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("PayPal returned an empty product response.");

        using var planRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/billing/plans");
        planRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        planRequest.Content = JsonContent.Create(new
        {
            product_id = product.Id,
            name = request.Name,
            billing_cycles = new[]
            {
                new
                {
                    frequency = new { interval_unit = MapInterval(request.BillingCycle), interval_count = 1 },
                    tenure_type = "REGULAR",
                    sequence = 1,
                    total_cycles = 0,
                    pricing_scheme = new { fixed_price = new { value = request.Amount.ToString("F2"), currency_code = request.Currency.ToUpperInvariant() } },
                },
            },
            payment_preferences = new { auto_bill_outstanding = true, payment_failure_threshold = 3 },
        });

        using var planResponse = await client.SendAsync(planRequest, cancellationToken);
        planResponse.EnsureSuccessStatusCode();
        var plan = await planResponse.Content.ReadFromJsonAsync<PayPalPlan>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("PayPal returned an empty plan response.");

        return new SubscriptionPriceResult(product.Id, plan.Id);
    }

    public async Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/billing/subscriptions");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpRequest.Headers.Add("PayPal-Request-Id", request.ReferenceId);
        httpRequest.Content = JsonContent.Create(new
        {
            plan_id = request.ProviderPriceId,
            custom_id = request.ReferenceId,
            subscriber = new { email_address = request.ProviderCustomerId },
            application_context = new { return_url = _options.ReturnUrl, cancel_url = _options.CancelUrl },
        });

        using var client = CreateClient();
        using var response = await client.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var subscription = await response.Content.ReadFromJsonAsync<PayPalSubscription>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("PayPal returned an empty subscription response.");

        var approveLink = subscription.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href;

        return new SubscriptionResult(subscription.Id, MapStatus(subscription.Status), DateTimeOffset.UtcNow, approveLink);
    }

    public async Task<SubscriptionResult> CancelSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/v1/billing/subscriptions/{providerSubscriptionId}/cancel");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpRequest.Content = JsonContent.Create(new { reason = "Cancelled by customer" });

        using var client = CreateClient();
        using var response = await client.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        return new SubscriptionResult(providerSubscriptionId, PaymentStatuses.Refunded, DateTimeOffset.UtcNow);
    }

    public async Task<SubscriptionWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        throw new NotSupportedException("PayPal webhook verification needs the original request headers - not implemented until PayPal is actually activated.");
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent([new KeyValuePair<string, string>("grant_type", "client_credentials")]),
        };
        var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        using var client = CreateClient();
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<PayPalTokenResponse>(cancellationToken: cancellationToken);

        return token?.AccessToken ?? throw new InvalidOperationException("PayPal did not return an access token.");
    }

    private static string MapInterval(string billingCycle) => billingCycle switch
    {
        BillingCycles.Annual => "YEAR",
        _ => "MONTH",
    };

    private static string MapStatus(string paypalStatus) => paypalStatus switch
    {
        "ACTIVE" => PaymentStatuses.Succeeded,
        "CANCELLED" or "EXPIRED" => PaymentStatuses.Failed,
        _ => PaymentStatuses.Pending,
    };

    private record PayPalTokenResponse([property: JsonPropertyName("access_token")] string AccessToken);

    private record PayPalProduct(string Id);

    private record PayPalPlan(string Id);

    private record PayPalSubscription(string Id, string Status, List<PayPalLink>? Links);

    private record PayPalLink(string Rel, string Href);
}
