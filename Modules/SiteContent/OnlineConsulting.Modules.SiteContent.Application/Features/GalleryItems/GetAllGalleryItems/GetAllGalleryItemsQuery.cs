using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
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
        var items = await itemRepository.GetListAsync(orderBy: q => q.OrderBy(x => x.DisplayOrder), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var links = await linkRepository.GetListAsync(size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var categories = await categoryRepository.GetListAsync(size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var categoriesById = categories.Items.ToDictionary(c => c.Id);
        var linksByItemId = links.Items.ToLookup(l => l.GalleryItemId);

        var response = items.Items
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
