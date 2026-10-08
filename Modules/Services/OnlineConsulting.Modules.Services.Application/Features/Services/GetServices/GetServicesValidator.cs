using FluentValidation;
using Core.ApplicationLayer.Requests.Page;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.GetServices;

public class GetServicesValidator : AbstractValidator<GetServicesQuery>
{
    public GetServicesValidator()
    {
        _ = RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
