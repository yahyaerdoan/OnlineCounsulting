using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.GetAllGalleryItemsPaged;

public record GetAllGalleryItemsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<GalleryItemResponse>>>;

public class GetAllGalleryItemsPagedHandler(IGalleryItemRepository itemRepository, IGalleryItemCategoryRepository linkRepository, IGalleryCategoryRepository categoryRepository)
    : IRequestHandler<GetAllGalleryItemsPagedQuery, OperationDataResult<Paginate<GalleryItemResponse>>>
{
    public async Task<OperationDataResult<Paginate<GalleryItemResponse>>> Handle(GetAllGalleryItemsPagedQuery request, CancellationToken cancellationToken)
    {
        var paged = await itemRepository.Query().ToDynamicPaginateAsync(request.PageRequest, request.DynamicQuery, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken);

        var itemIds = paged.Items.Select(x => x.Id).ToHashSet();
        var links = await linkRepository.GetListAsync(x => itemIds.Contains(x.GalleryItemId), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var categories = await categoryRepository.GetListAsync(size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var categoriesById = categories.Items.ToDictionary(c => c.Id);
        var linksByItemId = links.Items.ToLookup(l => l.GalleryItemId);

        var response = new Paginate<GalleryItemResponse>
        {
            Items =
            [
                .. paged.Items.Select(item =>
                {
                    var itemCategories = linksByItemId[item.Id]
                        .Where(link => categoriesById.ContainsKey(link.GalleryCategoryId))
                        .Select(link => GalleryCategoryResponse.FromDomain(categoriesById[link.GalleryCategoryId]))
                        .ToList();

                    return GalleryItemResponse.FromDomain(item, itemCategories);
                }),
            ],
            Index = paged.Index,
            Size = paged.Size,
            Count = paged.Count,
            Pages = paged.Pages,
        };

        return Result.Success(response, "Gallery items retrieved successfully.");
    }
}
