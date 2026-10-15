namespace OnlineConsulting.Modules.SiteContent.Application.Common;

/// <summary>Shared across every SiteContent entity - each one is a "not found" the same shape, so a single formatter covers them instead of six near-identical Messages classes.</summary>
public static class SiteContentMessages
{
    public const string NotFoundFormat = "{0} {1} was not found.";
    public const string LandingPageNotSet = "The landing page has not been set up yet.";
    public const string UnknownLandingSectionFormat = "Section must be one of: {0}.";
}
