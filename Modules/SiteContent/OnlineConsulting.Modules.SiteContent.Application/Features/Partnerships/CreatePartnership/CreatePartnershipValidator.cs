using FluentValidation;
using OnlineConsulting.Modules.SiteContent.Domain.Partnerships;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.CreatePartnership;

public class CreatePartnershipValidator : AbstractValidator<CreatePartnershipCommand>
{
    public CreatePartnershipValidator()
    {
        _ = RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Kind).Must(kind => PartnershipKinds.All.Contains(kind)).WithMessage("'Kind' must be Partner or Team.");
        _ = RuleFor(x => x.Title).NotEmpty().MinimumLength(2).MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        _ = RuleFor(x => x.Email).NotEmpty().When(x => x.Kind == PartnershipKinds.Partner);
        _ = RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Email));
        _ = RuleFor(x => x.CompanyName).NotEmpty().When(x => x.Kind == PartnershipKinds.Partner);
        _ = RuleFor(x => x.CompanyName).MaximumLength(200);
        _ = RuleFor(x => x.WebsiteUrl).NotEmpty().When(x => x.Kind == PartnershipKinds.Partner);
        _ = RuleFor(x => x.WebsiteUrl).MaximumLength(500).Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("'{PropertyName}' must be a valid URL.").When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));
    }
}
