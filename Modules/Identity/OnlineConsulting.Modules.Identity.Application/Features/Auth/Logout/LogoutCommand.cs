using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Logout;

public record LogoutCommand : IRequest<OperationResult>;

public class LogoutHandler(SignInManager<User> signInManager) : IRequestHandler<LogoutCommand, OperationResult>
{
    public async Task<OperationResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();
        return Result.Success("You've been logged out. See you again soon!");
    }
}
