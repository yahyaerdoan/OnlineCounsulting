using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServices;

public class GetServicesValidator : AbstractValidator<GetServicesQuery>
{
    public GetServicesValidator()
    {
        RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
