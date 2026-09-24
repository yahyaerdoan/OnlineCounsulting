namespace OnlineConsulting.Maui.Infrastructure.Api;

/// <summary>Api base URL for the native head, which has no Aspire service discovery.</summary>
public static class ApiEndpoint
{
    private const string EnvironmentVariableName = "API_BASE_URL";

#if ANDROID
    // Api's Development Kestrel cert covers 10.0.2.2 (see OnlineConsulting.Api/DevCerts/README.md)
    // and the Android app trusts it for that domain (network_security_config.xml) - so https works
    // here the same as everywhere else, no cleartext/mixed-content workarounds needed.
    private const string DevDefaultUrl = "https://10.0.2.2:7012";
#else
    private const string DevDefaultUrl = "https://localhost:7012";
#endif

    public static string BaseUrl =>
        Environment.GetEnvironmentVariable(EnvironmentVariableName) is { Length: > 0 } configured
            ? configured
            : DevDefaultUrl;
}
