using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ContactDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Contacts;

internal class UpdateContactValidator : AbstractValidator<UpdateContactDto>
{
    public UpdateContactValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email cannot be empty!")
            .NotNull().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Invalid email address format!")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters!")
            .Must(BeAValidDomain).WithMessage("Email must have a valid domain.");

        RuleFor(x => x.Phone)
           .NotEmpty().WithMessage("Phone cannot be empty!")
           .MinimumLength(5).WithMessage("Phone must be over 5 characters!");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address  cannot be empty!")
            .MinimumLength(5).WithMessage("Address  must be over 5 characters!");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

        RuleFor(x => x.WorkingHours)
            .NotEmpty().WithMessage("Working Hours cannot be empty!")
            .MinimumLength(5).WithMessage("Working Hours must be over 5 characters!");
    }

    /// <summary>Checks whether the email's domain is in the allowed list.</summary>
    private bool BeAValidDomain(string email)
    {
        var validDomains = new[] { "gmail.com", "yahoo.com", "outlook.com" };
        var domain = email.Split('@').LastOrDefault();
        return domain is not null && validDomains.Contains(domain);
    }
}
