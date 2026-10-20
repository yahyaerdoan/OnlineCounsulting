using Microsoft.AspNetCore.Components.Server.Circuits;

namespace OnlineConsulting.Maui.Web.Infrastructure.Api;

/// <summary>Refills <see cref="BrowserRequestContext"/> on every (re)connection.</summary>
public sealed class BrowserRequestCircuitHandler(IHttpContextAccessor httpContextAccessor, BrowserRequestContext browserRequest) : CircuitHandler
{
    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext is { } context)
        {
            browserRequest.Capture(context, canWriteCookies: false);
        }

        return Task.CompletedTask;
    }
}
