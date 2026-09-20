using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.ChangePassword;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<OperationResult>;

public class ChangePasswordHandler(UserManager<User> userManager) : IRequestHandler<ChangePasswordCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return UserBusinessRules.UserNotFound();
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            return Result.Success("Password changed successfully.");
        }

        // NewPassword's own complexity is already caught earlier by ChangePasswordValidator, so the
        // only failure UserManager can realistically still report here is the current password being
        // wrong - attribute it to that field instead of NewPassword.
        var isCurrentPasswordWrong = result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordMismatch));
        var field = isCurrentPasswordWrong ? nameof(request.CurrentPassword) : nameof(request.NewPassword);

        return OperationResult.Failure(new Dictionary<string, IReadOnlyList<string>>
        {
            [field] = [.. result.Errors.Select(e => e.Description)],
        });
    }
}
