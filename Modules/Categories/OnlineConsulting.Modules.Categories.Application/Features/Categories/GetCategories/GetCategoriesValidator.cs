using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Categories.Application.Features.Categories.GetCategories;

public class GetCategoriesValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesValidator()
    {
        _ = RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
