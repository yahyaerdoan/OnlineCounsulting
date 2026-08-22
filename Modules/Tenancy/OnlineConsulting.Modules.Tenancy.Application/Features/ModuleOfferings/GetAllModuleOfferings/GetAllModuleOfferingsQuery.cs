using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.GetAllModuleOfferings;

/// <summary>Platform-owner catalog listing - every module offering regardless of IsPubliclyVisible, unlike GetPublicModuleOfferingsQuery.</summary>
public record GetAllModuleOfferingsQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<ModuleOfferingAdminResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class GetAllModuleOfferingsHandler(IModuleOfferingRepository repository)
    : IRequestHandler<GetAllModuleOfferingsQuery, OperationDataResult<Paginate<ModuleOfferingAdminResponse>>>
{
    public async Task<OperationDataResult<Paginate<ModuleOfferingAdminResponse>>> Handle(GetAllModuleOfferingsQuery request, CancellationToken cancellationToken)
    {
        var offerings = await repository.GetListAsync(index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<ModuleOfferingAdminResponse>
        {
            Items = [.. offerings.Items.Select(ModuleOfferingAdminResponse.FromDomain)],
            Index = offerings.Index,
            Size = offerings.Size,
            Count = offerings.Count,
            Pages = offerings.Pages,
        };

        return Result.Success(response, "Module offerings retrieved successfully.");
    }
}
