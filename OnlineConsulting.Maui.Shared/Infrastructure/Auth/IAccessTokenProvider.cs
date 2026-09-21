namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Reads/writes the current session's token set - cookie claims on Web, keystore on MAUI.</summary>
public interface IAccessTokenProvider
{
    /// <summary>Gets the current session's stored tokens, or null if signed out.</summary>
    Task<TokenSet?> GetTokenSetAsync();

    /// <summary>Persists a token set for the current session.</summary>
    Task SetTokenSetAsync(TokenSet tokens);

    /// <summary>Removes any stored token set (sign-out).</summary>
    Task ClearAsync();
}
