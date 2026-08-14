using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.GetAllUsers;

public record GetAllUsersQuery : IRequest<OperationDataResult<List<UserResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [UsersOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, UsersOperationClaims.Read];
}

public class GetAllUsersHandler(UserManager<User> userManager) : IRequestHandler<GetAllUsersQuery, OperationDataResult<List<UserResponse>>>
{
    public async Task<OperationDataResult<List<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userManager.Users.ToListAsync(cancellationToken);
        if (users.Count == 0)
            return Result.NotFound<List<UserResponse>>(UserMessages.NoUserDataFound);

        var userResponses = new List<UserResponse>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userResponses.Add(new UserResponse
            {
                Id = user.Id,
                TenantId = user.TenantId,
                UserName = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Roles = [.. roles],
            });
        }

        return Result.Success(userResponses, "User data retrieved successfully.");
    }
}
