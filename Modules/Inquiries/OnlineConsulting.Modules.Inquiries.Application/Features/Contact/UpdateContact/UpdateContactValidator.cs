using FluentValidation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Contact.UpdateContact;

public class UpdateContactValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactValidator()
    {
        _ = RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        _ = RuleFor(x => x.Phone).NotEmpty().MaximumLength(50);
        _ = RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.WorkingHours).NotEmpty().MaximumLength(200);
    }
}
