using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.ListAboutUss;

public record ListAboutUssQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<AboutUsResponse>>>;

public class ListAboutUssHandler(IAboutUsRepository repository) : IRequestHandler<ListAboutUssQuery, OperationDataResult<Paginate<AboutUsResponse>>>
{
    public async Task<OperationDataResult<Paginate<AboutUsResponse>>> Handle(ListAboutUssQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<AboutUsResponse>
        {
            Items = [.. paged.Items.Select(AboutUsResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "About Us entries retrieved successfully.");
    }
}
