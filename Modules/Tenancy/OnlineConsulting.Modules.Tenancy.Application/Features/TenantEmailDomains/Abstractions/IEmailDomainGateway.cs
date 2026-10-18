namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;

/// <summary>Registers tenant email domains with the email provider, which signs mail sent from them.</summary>
public interface IEmailDomainGateway
{
    Task<EmailDomainSetup> AddAsync(string domain, CancellationToken cancellationToken = default);

    Task<EmailDomainSetup> GetAsync(string domain, CancellationToken cancellationToken = default);

    /// <summary>Asks the provider to verify the DNS records now; also allows the sender address once they pass.</summary>
    Task<EmailDomainSetup> AuthenticateAsync(string domain, string senderAddress, string senderName, CancellationToken cancellationToken = default);

    Task RemoveAsync(string domain, CancellationToken cancellationToken = default);
}
