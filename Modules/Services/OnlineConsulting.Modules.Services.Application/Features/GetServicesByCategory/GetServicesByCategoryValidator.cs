using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Services.Application.Features.GetServicesByCategory;

public class GetServicesByCategoryValidator : AbstractValidator<GetServicesByCategoryQuery>
{
    public GetServicesByCategoryValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
