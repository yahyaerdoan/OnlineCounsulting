using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.NewsletterDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Newsletters;

internal class CreateNewsletterValidator : AbstractValidator<CreateNewsletterDto>
{
    public CreateNewsletterValidator()
    {

    }
}
