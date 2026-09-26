using System.Security.Cryptography.X509Certificates;

namespace OnlineConsulting.Api.Configurations.Extensions;

/// <summary>Development-only Kestrel certificate selection: the Android emulator gets the DevCerts certificate covering 10.0.2.2, every other client keeps the trusted ASP.NET Core dev certificate.</summary>
public static class DevelopmentCertificates
{
    private const string AspNetHttpsDevCertificateOid = "1.3.6.1.4.1.311.84.1.1";
    private const string EmulatorCertificatePassword = "devcert123";

    /// <summary>No-op outside Development or when DevCerts/onlineconsulting-dev.pfx is absent, so Kestrel keeps its default dev certificate.</summary>
    public static void UseEmulatorCertificateWhenPresent(this WebApplicationBuilder builder)
    {
        var emulatorCertificatePath = Path.Combine(builder.Environment.ContentRootPath, "DevCerts", "onlineconsulting-dev.pfx");
        if (!builder.Environment.IsDevelopment() || !File.Exists(emulatorCertificatePath))
        {
            return;
        }

        var emulatorCertificate = X509CertificateLoader.LoadPkcs12FromFile(emulatorCertificatePath, EmulatorCertificatePassword);
        var localhostCertificate = FindAspNetCoreDevCertificate() ?? emulatorCertificate;

        builder.WebHost.ConfigureKestrel(options => options.ConfigureHttpsDefaults(https =>
            https.ServerCertificateSelector = (_, hostName) => string.IsNullOrEmpty(hostName) ? emulatorCertificate : localhostCertificate));
    }

    private static X509Certificate2? FindAspNetCoreDevCertificate()
    {
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        var now = DateTime.Now;
        return store.Certificates
            .Where(certificate => certificate.HasPrivateKey
                && certificate.NotBefore <= now
                && certificate.NotAfter > now
                && certificate.Extensions.Any(extension => extension.Oid?.Value == AspNetHttpsDevCertificateOid))
            .OrderByDescending(certificate => certificate.NotAfter)
            .FirstOrDefault();
    }
}
