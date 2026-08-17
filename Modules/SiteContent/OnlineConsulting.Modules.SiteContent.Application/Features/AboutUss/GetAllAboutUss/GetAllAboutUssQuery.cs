using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.GetAllAboutUss;

public record GetAllAboutUssQuery : IRequest<OperationDataResult<List<AboutUsResponse>>>;

public class GetAllAboutUssHandler(IAboutUsRepository repository) : IRequestHandler<GetAllAboutUssQuery, OperationDataResult<List<AboutUsResponse>>>
{
    public async Task<OperationDataResult<List<AboutUsResponse>>> Handle(GetAllAboutUssQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = entities.Items.Select(AboutUsResponse.FromDomain).ToList();

        return Result.Success(response, "About Us content retrieved successfully.");
    }
}
