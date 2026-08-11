using FluentValidation;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.ServiceImages;

internal class CreateServiceImageValidator : AbstractValidator<CreateServiceImageDto>
{
    public CreateServiceImageValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("Service cannot be empty!");

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
