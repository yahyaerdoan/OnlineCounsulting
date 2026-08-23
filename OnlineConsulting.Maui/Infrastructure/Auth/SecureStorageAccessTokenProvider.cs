using System.Text.Json;
using Microsoft.Maui.Storage;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;

namespace OnlineConsulting.Maui.Infrastructure.Auth;

/// <summary>Persists the session's token set in the platform keystore.</summary>
public class SecureStorageAccessTokenProvider : IAccessTokenProvider
{
    private const string StorageKey = "OnlineConsulting.Maui.tokens";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<TokenSet?> GetTokenSetAsync()
    {
        var json = await SecureStorage.Default.GetAsync(StorageKey);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<TokenSet>(json, JsonOptions);
    }

    public Task SetTokenSetAsync(TokenSet tokens) =>
        SecureStorage.Default.SetAsync(StorageKey, JsonSerializer.Serialize(tokens, JsonOptions));

    public Task ClearAsync()
    {
        SecureStorage.Default.Remove(StorageKey);
        return Task.CompletedTask;
    }
}
