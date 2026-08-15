namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;

/// <summary>Every known flag key and the value a tenant gets when no override row exists. New keys are added here as legacy sections (Partnerships, GalleryItem, ...) get migrated to modules and need a per-tenant on/off switch.</summary>
public static class FeatureFlagKeys
{
    public const string Partnerships = "Partnerships";

    public static readonly IReadOnlyDictionary<string, bool> Defaults = new Dictionary<string, bool>
    {
        [Partnerships] = true,
    };
}
