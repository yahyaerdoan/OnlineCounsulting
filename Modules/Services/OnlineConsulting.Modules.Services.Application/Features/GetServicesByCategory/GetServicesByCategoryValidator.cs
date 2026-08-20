using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServicesByCategory;

public class GetServicesByCategoryValidator : AbstractValidator<GetServicesByCategoryQuery>
{
    public GetServicesByCategoryValidator()
    {
        _ = RuleFor(x => x.CategoryId).NotEmpty();
        _ = RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
