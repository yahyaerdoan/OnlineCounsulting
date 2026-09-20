using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.ResetPassword;

public record ResetPasswordCommand(Guid UserId, string Token, string NewPassword) : IRequest<OperationResult>, ITransactionAddRequest;

public class ResetPasswordHandler(UserManager<User> userManager) : IRequestHandler<ResetPasswordCommand, OperationResult>
{
    private const string _invalidResetMessage = "This reset link is invalid or has expired, or the password doesn't meet requirements.";
    private const string _successMessage = "Your password has been reset. You can now sign in.";

    public async Task<OperationResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result.BadRequest(_invalidResetMessage);
        }

        var resetResult = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        return !resetResult.Succeeded
            ? Result.BadRequest(_invalidResetMessage)
            : Result.Success(_successMessage);
    }
}
