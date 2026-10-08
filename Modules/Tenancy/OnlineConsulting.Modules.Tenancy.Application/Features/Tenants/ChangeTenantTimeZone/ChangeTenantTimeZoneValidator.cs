using FluentValidation;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ChangeTenantTimeZone;

public class ChangeTenantTimeZoneValidator : AbstractValidator<ChangeTenantTimeZoneCommand>
{
    public ChangeTenantTimeZoneValidator()
    {
        _ = RuleFor(x => x.TimeZoneId).Must(BusinessTimeZones.IsKnown).WithMessage("Choose a valid time zone.");
    }
}
