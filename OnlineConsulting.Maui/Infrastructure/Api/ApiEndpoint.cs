namespace OnlineConsulting.Maui.Infrastructure.Api;

/// <summary>Api base URL for the native head, which has no Aspire service discovery.</summary>
public static class ApiEndpoint
{
    private const string EnvironmentVariableName = "API_BASE_URL";

#if ANDROID
    private const string DevDefaultUrl = "https://10.0.2.2:7012";
#else
    private const string DevDefaultUrl = "https://localhost:7012";
#endif

    public static string BaseUrl =>
        Environment.GetEnvironmentVariable(EnvironmentVariableName) is { Length: > 0 } configured
            ? configured
            : DevDefault;

    private static string DevDefault =>
        AppEnvironment.IsDevelopment
            ? DevDefaultUrl
            : throw new InvalidOperationException(
                $"'{EnvironmentVariableName}' environment variable must be set to the production Api base URL outside Development.");
}
