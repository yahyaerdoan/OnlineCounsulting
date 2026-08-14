using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Register;

public record RegisterCommand(string FirstName, string LastName, string UserName, string Email, string Password)
    : IRequest<OperationResult>, ITransactionAddRequest;

public class RegisterHandler(UserManager<User> userManager, RoleManager<Role> roleManager) : IRequestHandler<RegisterCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TenantId = TenantDefaults.DefaultTenantId,
            ImageUrl = "/Resource/LocalStorage/DefaultImages/defaultUserImage.png",
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result.Invalid([.. createResult.Errors.Select(e => e.Description)]);

        if (!await roleManager.RoleExistsAsync(GlobalOperationClaims.User))
            await roleManager.CreateAsync(new Role { Name = GlobalOperationClaims.User });

        var roleResult = await userManager.AddToRoleAsync(user, GlobalOperationClaims.User);
        if (!roleResult.Succeeded)
            return Result.InternalServerError($"{string.Join("; ", roleResult.Errors.Select(e => e.Description))} errors occurred while assigning the default role. Please try again later.");

        return Result.Created("The user has been successfully created.");
    }
}
