using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Theme;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>The business whose site or app this is (name and logo), loaded once per user (circuit) from the API.</summary>
public sealed class SiteBrand(IApiClient apiClient)
{
    private Task? _loading;
    private string _platformUrl = string.Empty;

    /// <summary>Raised after a reload, so the header, footer and titles show the new name and logo.</summary>
    public event Action? Changed;

    public string Name { get; private set; } = string.Empty;

    public string? LogoUrl { get; private set; }

    /// <summary>First letter or digit of the name, uppercased; stands in for a missing logo.</summary>
    public string Initial => Name.Where(char.IsLetterOrDigit).Select(c => char.ToUpperInvariant(c).ToString()).FirstOrDefault() ?? string.Empty;

    /// <summary>The logo, or a generated circle with <see cref="Initial"/>; null until the brand loads.</summary>
    public string? FaviconUrl => LogoUrl ?? (Initial.Length == 0 ? null : InitialIconDataUri(Initial));

    /// <summary>True on the platform's own site; pricing and signup exist only there.</summary>
    public bool IsPlatform { get; private set; } = true;

    /// <summary>A platform page as a link that works from any site: relative on the platform, absolute on a business's site.</summary>
    public string PlatformLink(string path) => IsPlatform ? path : $"{_platformUrl}{path}";

    public Task EnsureLoadedAsync() => _loading ??= LoadAsync();

    /// <summary>Fetches the brand again, e.g. after an admin saves new branding.</summary>
    public async Task ReloadAsync()
    {
        _loading = LoadAsync();
        await _loading;
        Changed?.Invoke();
    }

    private static string InitialIconDataUri(string initial) =>
        "data:image/svg+xml," + Uri.EscapeDataString(
            $"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64'><circle cx='32' cy='32' r='32' fill='{BrandPalette.Brand}'/>" +
            $"<text x='32' y='44' text-anchor='middle' font-family='Segoe UI,Arial,sans-serif' font-size='34' font-weight='600' fill='#FFFFFF'>{initial}</text></svg>");

    private async Task LoadAsync()
    {
        try
        {
            var result = await apiClient.GetAsync<TenantBrandingResponse>(ApiRoutes.Tenancy.Branding);
            if (result.IsSuccessful && result.ResultData is { } brand)
            {
                Name = brand.Name;
                IsPlatform = brand.IsPlatform;
                _platformUrl = brand.PlatformUrl;
                LogoUrl = brand.LogoMediaAssetId is Guid logoId ? await MediaUploadHelper.GetUrlAsync(apiClient, logoId) : null;
            }
        }
        catch (HttpRequestException)
        {
        }
    }
}
