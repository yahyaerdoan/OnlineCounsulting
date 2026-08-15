using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.CreateFooterInfo;

public class CreateFooterInfoValidator : AbstractValidator<CreateFooterInfoCommand>
{
    public CreateFooterInfoValidator()
    {
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
