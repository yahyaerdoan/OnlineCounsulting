using Microsoft.AspNetCore.Http.Connections.Client;
using OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

namespace OnlineConsulting.Maui.Infrastructure.Api;

/// <summary>Gives the live-updates hub connection the same debug-only dev-certificate handling as MauiProgram's HttpClient
/// handler, for both the negotiate request and the WebSocket. Compiled out of Release builds entirely.</summary>
public sealed class DevCertificateLiveUpdatesConfigurator : ILiveUpdatesTransportConfigurator
{
    public void Configure(HttpConnectionOptions options)
    {
#if DEBUG
        options.HttpMessageHandlerFactory = _ => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };
        options.WebSocketConfiguration = socket => socket.RemoteCertificateValidationCallback = (_, _, _, _) => true;
#endif
    }
}
