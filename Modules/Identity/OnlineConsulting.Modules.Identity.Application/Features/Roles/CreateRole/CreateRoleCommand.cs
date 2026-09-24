using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.CreateRole;

public record CreateRoleCommand(string Name, string? Description) : IRequest<OperationResult>, ISecureAddRequest
{
    /// <summary>Roles aren't tenant-scoped, so only Super Admin may create one - it's shared across tenants.</summary>
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    [JsonIgnore]
    public bool AllowTenantBypass => false;
}

public class CreateRoleHandler(RoleManager<Role> roleManager) : IRequestHandler<CreateRoleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new Role { Name = request.Name, Description = request.Description };

        var result = await roleManager.CreateAsync(role);

        return result.Succeeded
            ? Result.Created("The role has been successfully created.")
            : Result.BadRequest($"{string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))} errors occurred while saving the role. Please try again later.");
    }
}
