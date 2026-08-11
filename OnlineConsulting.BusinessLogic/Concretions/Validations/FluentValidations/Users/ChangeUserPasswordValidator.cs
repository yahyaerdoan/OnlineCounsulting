using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Users;

public class ChangeUserPasswordValidator : AbstractValidator<ChangePasswordUserDto>
{
    public ChangeUserPasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password cannot be empty!");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password cannot be empty!");
    }
}
