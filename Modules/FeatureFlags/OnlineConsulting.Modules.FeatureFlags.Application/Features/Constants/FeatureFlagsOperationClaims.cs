namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;

public static class FeatureFlagsOperationClaims
{
    public const string Admin = "featureflags.admin";
    public const string Read = "featureflags.read";
    public const string Write = "featureflags.write";
    public const string Add = "featureflags.add";
    public const string Update = "featureflags.update";
    public const string Delete = "featureflags.delete";

    public static readonly string[] All = [Admin, Read, Write, Add, Update, Delete];
}
