namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;

/// <summary>Record names as registrars expect them, relative to the customer's domain.</summary>
public static class DnsRecordNames
{
    /// <summary>"www" for "www.example.com", "@" for "example.com". Assumes a two-label domain, so "www.example.co.uk" gives "www.example".</summary>
    public static string Relative(string host)
    {
        var labels = host.Split('.');
        return labels.Length > 2 ? string.Join('.', labels[..^2]) : "@";
    }
}
