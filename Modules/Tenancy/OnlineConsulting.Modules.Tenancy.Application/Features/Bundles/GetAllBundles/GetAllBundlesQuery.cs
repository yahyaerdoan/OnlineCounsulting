using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.GetAllBundles;

/// <summary>Platform-owner catalog listing - every bundle regardless of IsPubliclyVisible, unlike GetPublicBundlesQuery.</summary>
public record GetAllBundlesQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<BundleAdminResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class GetAllBundlesHandler(IBundleRepository repository)
    : IRequestHandler<GetAllBundlesQuery, OperationDataResult<Paginate<BundleAdminResponse>>>
{
    public async Task<OperationDataResult<Paginate<BundleAdminResponse>>> Handle(GetAllBundlesQuery request, CancellationToken cancellationToken)
    {
        var bundles = await repository.GetListAsync(index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<BundleAdminResponse>
        {
            Items = [.. bundles.Items.Select(BundleAdminResponse.FromDomain)],
            Index = bundles.Index,
            Size = bundles.Size,
            Count = bundles.Count,
            Pages = bundles.Pages,
        };

        return Result.Success(response, "Bundles retrieved successfully.");
    }
}
