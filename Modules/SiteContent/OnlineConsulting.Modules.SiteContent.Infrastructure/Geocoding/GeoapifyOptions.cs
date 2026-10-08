namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Geocoding;

/// <summary>"Geoapify" config section (the same key Commerce's address autocomplete reads). An empty ApiKey falls back to Nominatim.</summary>
public sealed class GeoapifyOptions
{
    public const string SectionName = "Geoapify";

    public string? ApiKey { get; set; }
}
