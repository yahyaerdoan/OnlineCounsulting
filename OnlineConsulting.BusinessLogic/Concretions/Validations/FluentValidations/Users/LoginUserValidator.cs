using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Users;

internal class LoginUserValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("Username or email is required.");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}
