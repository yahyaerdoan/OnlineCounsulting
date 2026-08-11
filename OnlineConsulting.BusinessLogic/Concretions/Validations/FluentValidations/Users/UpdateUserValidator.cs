using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Users;

internal class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {

    }
}
