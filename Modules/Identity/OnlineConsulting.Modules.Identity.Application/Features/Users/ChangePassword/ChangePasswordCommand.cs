using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.ChangePassword;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<OperationResult>;

/// <summary>
/// Changes a user's password. New-password complexity is already covered by the validator, so any
/// UserManager failure here is attributed to an incorrect current password.
/// </summary>
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

        var isCurrentPasswordWrong = result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordMismatch));
        var field = isCurrentPasswordWrong ? nameof(request.CurrentPassword) : nameof(request.NewPassword);

        return OperationResult.Failure(new Dictionary<string, IReadOnlyList<string>>
        {
            [field] = [.. result.Errors.Select(e => e.Description)]
        });
    }
}
