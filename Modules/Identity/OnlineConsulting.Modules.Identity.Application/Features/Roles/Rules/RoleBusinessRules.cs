using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.Rules;

public static class RoleBusinessRules
{
    public static OperationResult NoRoleDataFound() => Result.NotFound(RoleMessages.NoRoleDataFound);
}
