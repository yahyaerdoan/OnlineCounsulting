using Microsoft.AspNetCore.Http.Connections.Client;

namespace OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

/// <summary>Optional host hook for the hub connection's transport (e.g. MAUI's debug-only dev-certificate handling).</summary>
public interface ILiveUpdatesTransportConfigurator
{
    void Configure(HttpConnectionOptions options);
}
