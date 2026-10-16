using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace OnlineConsulting.Maui.Web.Infrastructure.Hosting;

/// <summary>Restores the customer's domain as the request host when the edge Worker forwarded it, so tenant lookup, redirects and cookies use that domain.
/// Without the matching secret the headers are ignored, so a direct caller cannot pick a host.</summary>
public class EdgeHostMiddleware(RequestDelegate next, IOptions<EdgeHostOptions> options)
{
    public const string HostHeader = "X-Edge-Host";
    public const string SecretHeader = "X-Edge-Secret";

    public Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var secret = options.Value.Secret;
        var host = request.Headers[HostHeader].ToString();

        if (secret.Length > 0 && host.Length > 0 && IsSecret(request.Headers[SecretHeader].ToString(), secret))
        {
            request.Host = new HostString(host);
            request.Scheme = Uri.UriSchemeHttps;
        }

        _ = request.Headers.Remove(HostHeader);
        _ = request.Headers.Remove(SecretHeader);
        return next(context);
    }

    private static bool IsSecret(string candidate, string secret) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(candidate), Encoding.UTF8.GetBytes(secret));
}
