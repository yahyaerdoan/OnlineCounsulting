using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.UpdatePartnership;

public class UpdatePartnershipValidator : AbstractValidator<UpdatePartnershipCommand>
{
    public UpdatePartnershipValidator()
    {
        _ = RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        _ = RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(200);
        _ = RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        _ = RuleFor(x => x.WebsiteUrl).NotEmpty().Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("'{PropertyName}' must be a valid URL.");
    }
}
