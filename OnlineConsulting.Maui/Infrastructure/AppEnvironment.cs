namespace OnlineConsulting.Maui.Infrastructure;

public static class AppEnvironment
{
    private const string EnvironmentVariableName = "DOTNET_ENVIRONMENT";

    /// <summary>Whether the app is running in Development; defaults to true when the env var isn't set. Not build-config-aware - a Release-only guarantee needs its own #if DEBUG at the call site (see ApiEndpoint.DevDefault).</summary>
    public static bool IsDevelopment =>
        !(Environment.GetEnvironmentVariable(EnvironmentVariableName) is { Length: > 0 } value) || string.Equals(value, "Development", StringComparison.OrdinalIgnoreCase);
}
