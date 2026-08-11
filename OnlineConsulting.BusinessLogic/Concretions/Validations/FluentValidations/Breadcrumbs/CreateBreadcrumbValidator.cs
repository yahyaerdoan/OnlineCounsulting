using FluentValidation;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BreadcrumbDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Breadcrumbs;

public class CreateBreadcrumbValidator : AbstractValidator<CreateBreadcrumbDto>
{
    public CreateBreadcrumbValidator()
    {
        RuleFor(x => x.Title)
             .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
             .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

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
