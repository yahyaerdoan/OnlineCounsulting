using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Media.Application.Abstractions;
using OnlineConsulting.Modules.Media.Application.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Media.Application.Features.GetMediaAssets;

public record GetMediaAssetsQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<MediaAssetResponse>>>;

public class GetMediaAssetsHandler(IMediaAssetRepository repository)
    : IRequestHandler<GetMediaAssetsQuery, OperationDataResult<Paginate<MediaAssetResponse>>>
{
    public async Task<OperationDataResult<Paginate<MediaAssetResponse>>> Handle(GetMediaAssetsQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderByDescending(x => x.CreatedDate),
            index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        var response = new Paginate<MediaAssetResponse>
        {
            Items = [.. entities.Items.Select(MediaAssetResponse.FromDomain)],
            Index = entities.Index,
            Size = entities.Size,
            Count = entities.Count,
            Pages = entities.Pages,
        };

        return Result.Success(response, "Media assets retrieved successfully.");
    }
}
