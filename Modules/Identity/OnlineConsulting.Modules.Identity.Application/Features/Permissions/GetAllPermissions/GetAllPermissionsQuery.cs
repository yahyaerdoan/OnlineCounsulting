using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Authorization;
using MediatR;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Permissions.GetAllPermissions;

public record GetAllPermissionsQuery : IRequest<OperationDataResult<Dictionary<string, string[]>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [RolesOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, RolesOperationClaims.Read];
}

public class GetAllPermissionsHandler(IPermissionCatalog permissionCatalog) : IRequestHandler<GetAllPermissionsQuery, OperationDataResult<Dictionary<string, string[]>>>
{
    public Task<OperationDataResult<Dictionary<string, string[]>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken) =>
        Task.FromResult<OperationDataResult<Dictionary<string, string[]>>>(
            Result.Success(new Dictionary<string, string[]>(permissionCatalog.PermissionsByModule), "Permissions retrieved successfully."));
}
