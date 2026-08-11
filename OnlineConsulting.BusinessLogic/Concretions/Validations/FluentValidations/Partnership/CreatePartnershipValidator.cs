using FluentValidation;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Partnership;

internal class CreatePartnershipValidator : AbstractValidator<CreatePartnershipDto>
{
    public CreatePartnershipValidator()
    {
        RuleFor(x => x.Title)
          .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
          .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name cannot be empty!");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty!").EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty!");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty!");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description cannot be empty.")
            .MinimumLength(5).WithMessage("Description must be over 5 characters!");

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty().WithMessage(ValidationMessage.TheUrlNotEmpty)
            .MinimumLength(17).WithMessage(ValidationMessage.TheUrlMinimumLength)
            .Matches(ValidationMessage.TheUrlMatches)
            .WithMessage(ValidationMessage.TheUrlMatchesExample);

        RuleFor(x => x.Image)
            .NotEmpty().WithMessage(ValidationMessage.TheImageNotEmpty)
            .Must(IsValidImage).WithMessage(ValidationMessage.TheImageMustFormat);
    }
    private bool IsValidImage(IFormFile? image)
    {
        if (image is null)
            return false;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(extension);
    }
}
