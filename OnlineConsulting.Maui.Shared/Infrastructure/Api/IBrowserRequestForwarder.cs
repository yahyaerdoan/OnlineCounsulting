namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Adds the browser's host and guest id to Api calls; web only.</summary>
public interface IBrowserRequestForwarder
{
    void Apply(HttpRequestMessage request);

    /// <summary>Keeps a guest id the Api issued or refreshed.</summary>
    void Relay(HttpResponseMessage response);
}
