using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.ListGalleryItems;

public record ListGalleryItemsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<GalleryItemResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(GalleryItem.Id), nameof(GalleryItem.Description), nameof(GalleryItem.DisplayOrder)]);
}

public class ListGalleryItemsHandler(IGalleryItemRepository itemRepository, IGalleryItemCategoryRepository linkRepository, IGalleryCategoryRepository categoryRepository)
    : IRequestHandler<ListGalleryItemsQuery, OperationDataResult<Paginate<GalleryItemResponse>>>
{
    public async Task<OperationDataResult<Paginate<GalleryItemResponse>>> Handle(ListGalleryItemsQuery request, CancellationToken cancellationToken)
    {
        var paged = await itemRepository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var itemIds = paged.Items.Select(x => x.Id).ToHashSet();
        var links = await linkRepository.GetAllAsync(x => itemIds.Contains(x.GalleryItemId), cancellationToken: cancellationToken);
        var categories = await categoryRepository.GetAllAsync(cancellationToken: cancellationToken);

        var categoriesById = categories.ToDictionary(c => c.Id);
        var linksByItemId = links.ToLookup(l => l.GalleryItemId);

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
