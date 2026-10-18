using System.Text.RegularExpressions;
using FluentValidation;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.ConnectTenantEmailDomain;

public partial class ConnectTenantEmailDomainValidator : AbstractValidator<ConnectTenantEmailDomainCommand>
{
    public ConnectTenantEmailDomainValidator()
    {
        _ = RuleFor(x => x.Domain).NotEmpty().MaximumLength(253);
        _ = RuleFor(x => x.Domain).Must(domain => DomainPattern().IsMatch(TenantDomain.NormalizeHost(domain))).When(x => !string.IsNullOrWhiteSpace(x.Domain))
            .WithMessage(TenantEmailDomainMessages.InvalidDomain);
    }

    // A host name that doesn't start with "www.".
    [GeneratedRegex("^(?!www\\.)(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z]{2,63}$")]
    private static partial Regex DomainPattern();
}
