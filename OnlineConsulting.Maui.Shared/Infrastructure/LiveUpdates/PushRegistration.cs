using Microsoft.Extensions.Logging;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

/// <summary>Ties this device's push token to the signed-in user: registered after sign-in (and again if the provider
/// rotates it), removed before sign-out clears the session - otherwise the next person on this phone would keep getting
/// the previous user's notifications. A no-op when the host has no IPushTokenSource.</summary>
public sealed class PushRegistration : IDisposable
{
    private readonly IApiClient _apiClient;
    private readonly ILogger<PushRegistration> _logger;
    private readonly IPushTokenSource? _source;
    private string? _registeredToken;
    private bool _signedIn;

    public PushRegistration(IApiClient apiClient, ILogger<PushRegistration> logger, IPushTokenSource? source = null)
    {
        _apiClient = apiClient;
        _logger = logger;
        _source = source;
        if (_source is not null)
        {
            _source.TokenChanged += OnTokenChanged;
        }
    }

    /// <summary>Registers the current token for the signed-in user; repeated calls with an unchanged token do nothing.</summary>
    public async Task RegisterAsync(CancellationToken cancellationToken = default)
    {
        _signedIn = true;
        if (_source is null || await _source.GetTokenAsync(cancellationToken) is not { } token)
        {
            return;
        }

        await RegisterTokenAsync(token, cancellationToken);
    }

    /// <summary>Removes this device's token from the Api. Must run while the session's access token is still valid.</summary>
    public async Task UnregisterAsync(CancellationToken cancellationToken = default)
    {
        _signedIn = false;
        if (_registeredToken is not { } token)
        {
            return;
        }

        var result = await _apiClient.DeleteAsync(ApiRoutes.DeviceTokens.ById(token), cancellationToken);
        if (!result.IsSuccessful)
        {
            _logger.LogWarning("Removing the push token failed: {Message}", result.DisplayMessage);
        }

        _registeredToken = null;
    }

    private async Task RegisterTokenAsync(PushDeviceToken token, CancellationToken cancellationToken)
    {
        if (token.Token == _registeredToken)
        {
            return;
        }

        var result = await _apiClient.PostAsync(ApiRoutes.DeviceTokens.Base, new { token = token.Token, platform = token.Platform }, cancellationToken);
        if (result.IsSuccessful)
        {
            _registeredToken = token.Token;
        }
        else
        {
            _logger.LogWarning("Registering the push token failed: {Message}", result.DisplayMessage);
        }
    }

    private void OnTokenChanged(PushDeviceToken token)
    {
        if (_signedIn)
        {
            _ = RegisterTokenAsync(token, CancellationToken.None);
        }
    }

    public void Dispose()
    {
        if (_source is not null)
        {
            _source.TokenChanged -= OnTokenChanged;
        }
    }
}
