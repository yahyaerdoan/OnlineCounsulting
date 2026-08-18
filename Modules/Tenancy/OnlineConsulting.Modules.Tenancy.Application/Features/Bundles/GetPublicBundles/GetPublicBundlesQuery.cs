using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.GetPublicBundles;

/// <summary>Public, no login required - pricing-page data source for the signup form's bundle shortcuts. Only IsPubliclyVisible bundles.</summary>
public record GetPublicBundlesQuery : IRequest<OperationDataResult<List<BundleResponse>>>;

public class GetPublicBundlesHandler(IBundleRepository bundleRepository)
    : IRequestHandler<GetPublicBundlesQuery, OperationDataResult<List<BundleResponse>>>
{
    public async Task<OperationDataResult<List<BundleResponse>>> Handle(GetPublicBundlesQuery request, CancellationToken cancellationToken)
    {
        var bundles = await bundleRepository.GetListAsync(
            b => b.IsPubliclyVisible, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var response = bundles.Items.Select(BundleResponse.FromDomain).ToList();

        return Result.Success(response, "Bundles retrieved successfully.");
    }
}
