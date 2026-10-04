using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Contracts;
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
        var bundles = await bundleRepository.GetAllAsync(b => b.IsPubliclyVisible, cancellationToken: cancellationToken);

        var response = bundles.Select(BundleResponse.FromDomain).ToList();

        return Result.Success(response, "Bundles retrieved successfully.");
    }
}
