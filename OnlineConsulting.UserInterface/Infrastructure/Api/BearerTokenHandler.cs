using System.Net.Http.Headers;

namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>Attaches the session-stored Api access token to every outgoing IApiClient request; anonymous requests just go out without one.</summary>
public class BearerTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = httpContextAccessor.HttpContext?.Session.GetString(ApiSessionKeys.AccessToken);
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
