namespace OnlineConsulting.Modules.Commerce.Infrastructure.Addresses;

/// <summary>"Geoapify" config section. An empty ApiKey turns suggestions off; the address form still works by hand.</summary>
public sealed class GeoapifyOptions
{
    public const string SectionName = "Geoapify";

    public string? ApiKey { get; set; }
}
