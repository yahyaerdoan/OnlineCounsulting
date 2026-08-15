using DnsClient;
using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.MessageDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Messages;

internal class CreateMessageValidator : AbstractValidator<CreateMessageDto>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Please enter your first name.")
            .MaximumLength(50).WithMessage("First name can't be longer than 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Please enter your last name.")
            .MaximumLength(50).WithMessage("Last name can't be longer than 50 characters.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Please enter your email address.")
            .NotNull().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Hmm... that doesn't look like a valid email.")
            .MaximumLength(255).WithMessage("Email address is too long (max 255 characters).")
            .Must(BeAValidDomain).WithMessage("Please use a common email provider like Gmail, Yahoo, or Outlook.");
        // .Must(HaveValidMxRecord).WithMessage("The email provider domain doesn't appear to accept email.");


        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Please enter a subject for your message.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Don't forget to write your message.");
    }

    /// <summary>Checks whether the email's domain is in the allowed list.</summary>
    private bool BeAValidDomain(string email)
    {
        var validDomains = new[] { "gmail.com", "yahoo.com", "outlook.com" };
        var domain = email.Split('@').LastOrDefault();
        return domain is not null && validDomains.Contains(domain);
    }

    private static bool HaveValidMxRecord(string email)
    {
        try
        {
            var domain = email.Split('@').Last();
            var lookup = new LookupClient();
            var result = lookup.QueryAsync(domain, QueryType.MX).GetAwaiter().GetResult();
            return result.Answers.MxRecords().Any();
        }
        catch
        {
            return false;
        }
    }
}
