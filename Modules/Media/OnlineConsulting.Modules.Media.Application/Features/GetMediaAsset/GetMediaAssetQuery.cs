using MediatR;
using OnlineConsulting.Modules.Media.Application.Abstractions;
using OnlineConsulting.Modules.Media.Application.Contracts;
using OnlineConsulting.Modules.Media.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Media.Application.Features.GetMediaAsset;

public record GetMediaAssetQuery(Guid Id) : IRequest<OperationDataResult<MediaAssetResponse>>;

public class GetMediaAssetHandler(IMediaAssetRepository repository) : IRequestHandler<GetMediaAssetQuery, OperationDataResult<MediaAssetResponse>>
{
    public async Task<OperationDataResult<MediaAssetResponse>> Handle(GetMediaAssetQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, enableTracking: false, cancellationToken: cancellationToken);

        return entity is null
            ? Result.NotFound<MediaAssetResponse>(string.Format(MediaMessages.NotFoundFormat, request.Id))
            : Result.Success(MediaAssetResponse.FromDomain(entity), "Media asset retrieved successfully.");
    }
}
