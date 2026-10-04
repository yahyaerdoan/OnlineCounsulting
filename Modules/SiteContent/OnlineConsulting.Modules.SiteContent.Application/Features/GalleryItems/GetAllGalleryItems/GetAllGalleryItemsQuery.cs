using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.GetAllGalleryItems;

/// <summary>Public - no login required, matches GetAllTestimonialsQuery/GetAllPartnershipsQuery.</summary>
public record GetAllGalleryItemsQuery : IRequest<OperationDataResult<List<GalleryItemResponse>>>;

public class GetAllGalleryItemsHandler(IGalleryItemRepository itemRepository, IGalleryItemCategoryRepository linkRepository, IGalleryCategoryRepository categoryRepository)
    : IRequestHandler<GetAllGalleryItemsQuery, OperationDataResult<List<GalleryItemResponse>>>
{
    public async Task<OperationDataResult<List<GalleryItemResponse>>> Handle(GetAllGalleryItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await itemRepository.GetAllAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), cancellationToken: cancellationToken);
        var links = await linkRepository.GetAllAsync(cancellationToken: cancellationToken);
        var categories = await categoryRepository.GetAllAsync(cancellationToken: cancellationToken);

        var categoriesById = categories.ToDictionary(c => c.Id);
        var linksByItemId = links.ToLookup(l => l.GalleryItemId);

        var response = items
            .Select(item =>
            {
                var itemCategories = linksByItemId[item.Id]
                    .Where(link => categoriesById.ContainsKey(link.GalleryCategoryId))
                    .Select(link => GalleryCategoryResponse.FromDomain(categoriesById[link.GalleryCategoryId]))
                    .ToList();

                return GalleryItemResponse.FromDomain(item, itemCategories);
            })
            .ToList();

        return Result.Success(response, "Gallery items retrieved successfully.");
    }
}
