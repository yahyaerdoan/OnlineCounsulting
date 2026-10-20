using Microsoft.Extensions.Caching.Memory;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;

namespace OnlineConsulting.Maui.Web.Infrastructure.Auth;

/// <summary>In-process; a restart forgets it.</summary>
public sealed class MemoryRotatedTokenStore(IMemoryCache cache) : IRotatedTokenStore
{
    // Matches the Api's refresh token lifetime.
    private static readonly TimeSpan EntryLifetime = TimeSpan.FromDays(7);

    // Cycle guard.
    private const int MaxHops = 1000;

    public void Record(string oldRefreshToken, TokenSet replacement) =>
        cache.Set(Key(oldRefreshToken), replacement, EntryLifetime);

    public TokenSet? FindLatest(string refreshToken)
    {
        TokenSet? latest = null;
        var current = refreshToken;

        for (var hop = 0; hop < MaxHops && cache.TryGetValue<TokenSet>(Key(current), out var next) && next is not null; hop++)
        {
            latest = next;
            current = next.RefreshToken;
        }

        return latest;
    }

    private static string Key(string refreshToken) => $"rotated-refresh-token:{refreshToken}";
}
