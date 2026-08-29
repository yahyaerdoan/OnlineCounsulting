namespace OnlineConsulting.Api.Common;

/// <summary>Makes a local-storage relative URL absolute so other origins can load it. Already-absolute (S3/GCS/Azure) URLs pass through unchanged.</summary>
public static class MediaUrlResolver
{
    public static string Resolve(string url, HttpContext httpContext) =>
        url.StartsWith('/') ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{url}" : url;
}
