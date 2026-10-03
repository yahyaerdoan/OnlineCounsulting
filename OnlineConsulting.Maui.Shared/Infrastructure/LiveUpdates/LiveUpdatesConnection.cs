using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;

namespace OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;

/// <summary>Signed-in user's connection to the Api's /hubs/user-updates. A "DataChanged(topic)" signal is forwarded to
/// DataChangeNotifier; after a reconnect everything is reloaded, since signals sent while disconnected are lost.
/// If the Api is unreachable at start, it keeps retrying with backoff - resume/pull refresh still work meanwhile.</summary>
public sealed class LiveUpdatesConnection(
    IApiClient apiClient,
    IAccessTokenProvider tokenProvider,
    TokenRefresher tokenRefresher,
    DataChangeNotifier notifier,
    ILogger<LiveUpdatesConnection> logger,
    ILiveUpdatesTransportConfigurator? transportConfigurator = null) : IAsyncDisposable
{
    private const string HubPath = "hubs/user-updates";
    private const string DataChangedMethod = "DataChanged";
    private static readonly TimeSpan[] StartRetryDelays = [TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1)];
    private static readonly TimeSpan TokenRefreshBuffer = TimeSpan.FromSeconds(30);

    private readonly SemaphoreSlim _gate = new(1, 1);
    private HubConnection? _connection;
    private CancellationTokenSource? _startLoop;

    /// <summary>Connects if not already connected (idempotent); returns without waiting for a failed start's retries.</summary>
    public async Task StartAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (_connection is not null || apiClient.PublicBaseAddress is not { } baseAddress)
            {
                return;
            }

            _connection = Build(new Uri(baseAddress, HubPath));
            _startLoop = new CancellationTokenSource();
            _ = StartWithRetryAsync(_connection, _startLoop.Token);
        }
        finally
        {
            _ = _gate.Release();
        }
    }

    /// <summary>Disconnects (sign-out) - the next StartAsync builds a fresh connection with the new user's token.</summary>
    public async Task StopAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (_startLoop is not null)
            {
                await _startLoop.CancelAsync();
                _startLoop.Dispose();
                _startLoop = null;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }
        finally
        {
            _ = _gate.Release();
        }
    }

    private HubConnection Build(Uri hubUrl)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = GetAccessTokenAsync;
                transportConfigurator?.Configure(options);
            })
            .WithAutomaticReconnect()
            .Build();

        _ = connection.On<string>(DataChangedMethod, topic =>
            Enum.TryParse<DataTopic>(topic, ignoreCase: true, out var dataTopic) ? notifier.NotifyAsync(dataTopic) : Task.CompletedTask);
        connection.Reconnected += _ => notifier.NotifyAsync(DataTopic.All);
        return connection;
    }

    private async Task StartWithRetryAsync(HubConnection connection, CancellationToken cancellationToken)
    {
        for (var attempt = 0; !cancellationToken.IsCancellationRequested; attempt++)
        {
            try
            {
                await connection.StartAsync(cancellationToken);
                return;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                var delay = StartRetryDelays[Math.Min(attempt, StartRetryDelays.Length - 1)];
                logger.LogInformation(exception, "Live updates connection failed; retrying in {Delay}.", delay);
                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }
    }

    /// <summary>Same token rules as ApiClient: refresh when close to expiry, so a (re)connect never presents a stale token.</summary>
    private async Task<string?> GetAccessTokenAsync()
    {
        var tokens = await tokenProvider.GetTokenSetAsync();
        if (tokens is not null && tokens.IsNearExpiry(TokenRefreshBuffer))
        {
            tokens = await tokenRefresher.RefreshAsync(tokens, CancellationToken.None) ?? tokens;
        }

        return tokens?.AccessToken;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _gate.Dispose();
    }
}
