using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.GetBundleById;

public record GetBundleByIdQuery(Guid Id) : IRequest<OperationDataResult<BundleAdminResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class GetBundleByIdHandler(IBundleRepository repository) : IRequestHandler<GetBundleByIdQuery, OperationDataResult<BundleAdminResponse>>
{
    public async Task<OperationDataResult<BundleAdminResponse>> Handle(GetBundleByIdQuery request, CancellationToken cancellationToken)
    {
        var bundle = await repository.GetAsync(b => b.Id == request.Id, cancellationToken: cancellationToken);

        return bundle is null
            ? Result.NotFound<BundleAdminResponse>(string.Format(BundleMessages.BundleNotFoundFormat, request.Id))
            : Result.Success(BundleAdminResponse.FromDomain(bundle), "Bundle retrieved successfully.");
    }
}
