namespace OnlineConsulting.Storage;

/// <summary>Bound from the "Storage:Local" config section.</summary>
public class LocalStorageOptions
{
    /// <summary>Absolute path files are written to - defaults to the API host's own wwwroot/media so UseStaticFiles() serves them directly.</summary>
    public string RootPath { get; set; } = string.Empty;

    /// <summary>URL prefix returned to callers, e.g. "/media" - must match the static files mapping for RootPath.</summary>
    public string PublicPathPrefix { get; set; } = "/media";
}
