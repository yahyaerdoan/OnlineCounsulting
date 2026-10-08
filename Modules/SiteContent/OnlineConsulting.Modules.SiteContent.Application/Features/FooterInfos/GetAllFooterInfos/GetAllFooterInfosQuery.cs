using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.GetAllFooterInfos;

public record GetAllFooterInfosQuery : IRequest<OperationDataResult<List<FooterInfoResponse>>>;

public class GetAllFooterInfosHandler(IFooterInfoRepository repository) : IRequestHandler<GetAllFooterInfosQuery, OperationDataResult<List<FooterInfoResponse>>>
{
    public async Task<OperationDataResult<List<FooterInfoResponse>>> Handle(GetAllFooterInfosQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var response = entities.Select(FooterInfoResponse.FromDomain).ToList();

        return Result.Success(response, "Footer info retrieved successfully.");
    }
}
