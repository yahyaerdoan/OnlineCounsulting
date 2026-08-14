using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Register;

public record RegisterCommand(string FirstName, string LastName, string UserName, string Email, string Password)
    : IRequest<OperationResult>;

public class RegisterHandler(UserManager<User> userManager, RoleManager<Role> roleManager) : IRequestHandler<RegisterCommand, OperationResult>
{
    private const string DefaultRoleName = "User";

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

        if (!await roleManager.RoleExistsAsync(DefaultRoleName))
            await roleManager.CreateAsync(new Role { Name = DefaultRoleName });

        var roleResult = await userManager.AddToRoleAsync(user, DefaultRoleName);
        if (!roleResult.Succeeded)
            return Result.Failure("Partial failure",
                $"User created, but role assignment failed: {string.Join("; ", roleResult.Errors.Select(e => e.Description))}",
                ResultHandler.Core.Enums.ResultStatus.InternalServerError);

        return Result.Created("The user has been successfully created.");
    }
}
