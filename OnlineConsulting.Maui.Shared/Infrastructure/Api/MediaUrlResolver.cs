namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Turns a storage-relative media Url (e.g. "/uploads/x.jpg") into an absolute one rooted at
/// the Api's own origin - required in the native MAUI WebView, whose page origin is a local virtual
/// host and never matches the Api's, so a relative Url resolves to nothing and renders as a broken
/// image. Already-absolute Urls (Azure/S3/GCS providers, or any http(s) Url) pass through unchanged.</summary>
public static class MediaUrlResolver
{
    public static string? Resolve(IApiClient apiClient, string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            return url;
        }

        return apiClient.BaseAddress is null ? url : new Uri(apiClient.BaseAddress, url).ToString();
    }
}
