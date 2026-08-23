namespace OnlineConsulting.Maui.Infrastructure;

public static class AppEnvironment
{
    private const string EnvironmentVariableName = "DOTNET_ENVIRONMENT";

    // Env var wins where platforms support setting one (Windows dev/QA); everywhere else
    // (Android/iOS have no way to inject one into the deployed app) falls back to build config.
    public static bool IsDevelopment =>
        Environment.GetEnvironmentVariable(EnvironmentVariableName) is { Length: > 0 } value
            ? string.Equals(value, "Development", StringComparison.OrdinalIgnoreCase)
            : DefaultIsDevelopment;

#if DEBUG
    private const bool DefaultIsDevelopment = true;
#else
    private const bool DefaultIsDevelopment = false;
#endif
}
