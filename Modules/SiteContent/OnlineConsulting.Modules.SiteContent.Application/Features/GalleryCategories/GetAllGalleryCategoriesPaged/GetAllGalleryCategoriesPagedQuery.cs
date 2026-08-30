using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.GetAllGalleryCategoriesPaged;

public record GetAllGalleryCategoriesPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<GalleryCategoryResponse>>>;

public class GetAllGalleryCategoriesPagedHandler(IGalleryCategoryRepository repository)
    : IRequestHandler<GetAllGalleryCategoriesPagedQuery, OperationDataResult<Paginate<GalleryCategoryResponse>>>
{
    public async Task<OperationDataResult<Paginate<GalleryCategoryResponse>>> Handle(GetAllGalleryCategoriesPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.Name, tieBreaker: x => x.Id, cancellationToken);

        var response = new Paginate<GalleryCategoryResponse>
        {
            Items = [.. paged.Items.Select(GalleryCategoryResponse.FromDomain)],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Gallery categories retrieved successfully.");
    }
}
