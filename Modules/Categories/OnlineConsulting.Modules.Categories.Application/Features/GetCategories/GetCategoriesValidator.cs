using FluentValidation;
using OnlineConsulting.SharedKernel.Validation;

namespace OnlineConsulting.Modules.Categories.Application.Features.GetCategories;

public class GetCategoriesValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesValidator()
    {
        RuleFor(x => x.PageRequest).SetValidator(new PageRequestValidator());
    }
}
