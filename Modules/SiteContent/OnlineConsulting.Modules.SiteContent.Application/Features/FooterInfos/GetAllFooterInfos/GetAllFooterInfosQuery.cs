using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.GetAllFooterInfos;

public record GetAllFooterInfosQuery : IRequest<OperationDataResult<List<FooterInfoResponse>>>;

public class GetAllFooterInfosHandler(IFooterInfoRepository repository) : IRequestHandler<GetAllFooterInfosQuery, OperationDataResult<List<FooterInfoResponse>>>
{
    public async Task<OperationDataResult<List<FooterInfoResponse>>> Handle(GetAllFooterInfosQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(FooterInfoResponse.FromDomain).ToList();

        return Result.Success(response, "Footer info retrieved successfully.");
    }
}
