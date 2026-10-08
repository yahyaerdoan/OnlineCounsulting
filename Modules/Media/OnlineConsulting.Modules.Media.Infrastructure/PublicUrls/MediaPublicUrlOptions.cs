namespace OnlineConsulting.Modules.Media.Infrastructure.PublicUrls;

/// <summary>Bound from the Media config section.</summary>
public class MediaPublicUrlOptions
{
    /// <summary>Origin that serves locally stored files (the Api's public address), prefixed to relative URLs; cloud storage URLs are already absolute.</summary>
    public string PublicOrigin { get; set; } = string.Empty;
}
