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

public class ListGalleryItemsHandler(IGalleryItemRepository itemRepository, IGalleryCategoryRepository categoryRepository)
    : IRequestHandler<ListGalleryItemsQuery, OperationDataResult<Paginate<GalleryItemResponse>>>
{
    public async Task<OperationDataResult<Paginate<GalleryItemResponse>>> Handle(ListGalleryItemsQuery request, CancellationToken cancellationToken)
    {
        var paged = await itemRepository.QueryWithCategories().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.DisplayOrder, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

        var categoryIds = paged.Items.SelectMany(x => x.CategoryIds).Distinct().ToList();
        var categories = categoryIds.Count == 0 ? [] : await categoryRepository.GetAllAsync(c => categoryIds.Contains(c.Id), cancellationToken: cancellationToken);

        var categoriesById = categories.ToDictionary(c => c.Id);

        var response = new Paginate<GalleryItemResponse>
        {
            Items =
            [
                .. paged.Items.Select(item =>
                {
                    var itemCategories = item.CategoryIds
                        .Where(categoriesById.ContainsKey)
                        .Select(id => GalleryCategoryResponse.FromDomain(categoriesById[id]))
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
