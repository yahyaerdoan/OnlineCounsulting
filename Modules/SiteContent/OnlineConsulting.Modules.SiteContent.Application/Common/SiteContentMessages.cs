namespace OnlineConsulting.Modules.SiteContent.Application.Common;

/// <summary>Shared across every SiteContent entity - each one is a "not found" the same shape, so a single formatter covers them instead of six near-identical Messages classes.</summary>
public static class SiteContentMessages
{
    public const string NotFoundFormat = "{0} {1} was not found.";
}
