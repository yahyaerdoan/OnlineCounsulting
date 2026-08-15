using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.UpdateFooterInfo;

public class UpdateFooterInfoValidator : AbstractValidator<UpdateFooterInfoCommand>
{
    public UpdateFooterInfoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
