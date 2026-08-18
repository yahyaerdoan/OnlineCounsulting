using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.GetPublicModuleOfferings;

/// <summary>Public, no login required - pricing-page data source for the signup form. Only IsPubliclyVisible offerings, same catalog gate SignUpTenantCommand/AddModuleCommand use.</summary>
public record GetPublicModuleOfferingsQuery : IRequest<OperationDataResult<List<ModuleOfferingResponse>>>;

public class GetPublicModuleOfferingsHandler(IModuleOfferingRepository moduleOfferingRepository)
    : IRequestHandler<GetPublicModuleOfferingsQuery, OperationDataResult<List<ModuleOfferingResponse>>>
{
    public async Task<OperationDataResult<List<ModuleOfferingResponse>>> Handle(GetPublicModuleOfferingsQuery request, CancellationToken cancellationToken)
    {
        var offerings = await moduleOfferingRepository.GetListAsync(
            m => m.IsPubliclyVisible, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var response = offerings.Items.Select(ModuleOfferingResponse.FromDomain).ToList();

        return Result.Success(response, "Module offerings retrieved successfully.");
    }
}
