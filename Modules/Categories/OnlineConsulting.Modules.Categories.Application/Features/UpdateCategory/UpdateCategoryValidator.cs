using FluentValidation;

namespace OnlineConsulting.Modules.Categories.Application.Features.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Icon).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.IconColor).Matches("^#[0-9A-Fa-f]{6}$").When(x => x.IconColor is not null);
    }
}
