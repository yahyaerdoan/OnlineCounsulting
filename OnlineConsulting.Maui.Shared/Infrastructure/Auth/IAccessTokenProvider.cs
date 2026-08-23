namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Reads/writes the current session's token set - cookie claims on Web, keystore on MAUI.</summary>
public interface IAccessTokenProvider
{
    Task<TokenSet?> GetTokenSetAsync();

    Task SetTokenSetAsync(TokenSet tokens);

    Task ClearAsync();
}
