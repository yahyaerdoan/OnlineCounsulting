using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;

public static class UserBusinessRules
{
    public static OperationResult UserNotFound() => Result.NotFound(UserMessages.UserNotFound);

    public static OperationResult UserNotFoundOrInvalidData() => Result.BadRequest(UserMessages.UserNotFoundOrInvalidData);

    public static OperationResult NoUserDataFound() => Result.NotFound(UserMessages.NoUserDataFound);

    public static OperationResult NotAuthorizedForOtherTenant() => Result.Forbidden(UserMessages.NotAuthorizedForOtherTenant);
}
