using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.GetModuleOfferingById;

public record GetModuleOfferingByIdQuery(Guid Id) : IRequest<OperationDataResult<ModuleOfferingAdminResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    [JsonIgnore]
    public bool AllowTenantBypass => false;
}

public class GetModuleOfferingByIdHandler(IModuleOfferingRepository repository) : IRequestHandler<GetModuleOfferingByIdQuery, OperationDataResult<ModuleOfferingAdminResponse>>
{
    public async Task<OperationDataResult<ModuleOfferingAdminResponse>> Handle(GetModuleOfferingByIdQuery request, CancellationToken cancellationToken)
    {
        var offering = await repository.GetAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);

        return offering is null
            ? Result.NotFound<ModuleOfferingAdminResponse>(string.Format(ModuleOfferingMessages.ModuleOfferingNotFoundFormat, request.Id))
            : Result.Success(ModuleOfferingAdminResponse.FromDomain(offering), "Module offering retrieved successfully.");
    }
}
