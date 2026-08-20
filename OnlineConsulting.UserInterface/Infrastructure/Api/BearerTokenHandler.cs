using System.Net.Http.Headers;

namespace OnlineConsulting.UserInterface.Infrastructure.Api;

/// <summary>Attaches the current user's Api access token (session-stored at login) to every outgoing IApiClient request. Anonymous requests simply go out without an Authorization header - the Api's own [AllowAnonymous]/public endpoints handle that, everything else correctly comes back 401.</summary>
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
