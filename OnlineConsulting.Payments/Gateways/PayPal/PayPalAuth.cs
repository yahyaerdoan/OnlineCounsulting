using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Payments.Gateways.PayPal;

internal static class PayPalAuth
{
    public static async Task<string> GetAccessTokenAsync(HttpClient client, PayPalOptions options, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent([new KeyValuePair<string, string>("grant_type", "client_credentials")]),
        };
        var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{options.ClientId}:{options.ClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<PayPalTokenResponse>(cancellationToken: cancellationToken);

        return token?.AccessToken ?? throw new InvalidOperationException("PayPal did not return an access token.");
    }

    private record PayPalTokenResponse([property: JsonPropertyName("access_token")] string AccessToken);
}

internal record PayPalLink(string Rel, string Href);
