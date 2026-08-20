using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServices;

public class GetServicesValidator : AbstractValidator<GetServicesQuery>
{
    public GetServicesValidator()
    {
        _ = RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
