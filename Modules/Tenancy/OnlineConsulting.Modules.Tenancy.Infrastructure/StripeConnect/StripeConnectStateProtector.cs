using Microsoft.AspNetCore.DataProtection;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.StripeConnect;

/// <summary>Payload is "{tenantId}|{returnUrl}", protected (encrypted + signed) rather than merely encoded, so a tampered or expired token fails to unprotect instead of silently decoding to attacker-chosen values.</summary>
public class StripeConnectStateProtector : IStripeConnectStateProtector
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(15);

    private readonly ITimeLimitedDataProtector _protector;

    public StripeConnectStateProtector(IDataProtectionProvider dataProtectionProvider) =>
        _protector = dataProtectionProvider.CreateProtector(nameof(StripeConnectStateProtector)).ToTimeLimitedDataProtector();

    public string Protect(Guid tenantId, string returnUrl) =>
        _protector.Protect($"{tenantId}|{returnUrl}", Lifetime);

    public (Guid TenantId, string ReturnUrl)? Unprotect(string state)
    {
        string payload;

        try
        {
            payload = _protector.Unprotect(state);
        }
        catch (Exception)
        {
            return null;
        }

        var separatorIndex = payload.IndexOf('|');

        return separatorIndex > 0 && Guid.TryParse(payload[..separatorIndex], out var tenantId)
            ? (tenantId, payload[(separatorIndex + 1)..])
            : null;
    }
}
