using System.Text.RegularExpressions;
using FluentValidation;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.ConnectTenantDomain;

public partial class ConnectTenantDomainValidator : AbstractValidator<ConnectTenantDomainCommand>
{
    public ConnectTenantDomainValidator()
    {
        _ = RuleFor(x => x.Host).NotEmpty().MaximumLength(253).WithName("Domain");
        _ = RuleFor(x => x.Host).Must(host => HostPattern().IsMatch(TenantDomain.NormalizeHost(host))).When(x => !string.IsNullOrWhiteSpace(x.Host))
            .WithMessage(TenantDomainMessages.InvalidHost);
    }

    // Labels of letters, digits and inner hyphens, ending in a letters-only TLD.
    [GeneratedRegex("^(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z]{2,63}$")]
    private static partial Regex HostPattern();
}
