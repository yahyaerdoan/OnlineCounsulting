using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.GetAllGalleryItems;

/// <summary>Public - no login required, matches GetAllTestimonialsQuery/GetAllPartnershipsQuery.</summary>
public record GetAllGalleryItemsQuery : IRequest<OperationDataResult<List<GalleryItemResponse>>>;

public class GetAllGalleryItemsHandler(IGalleryItemRepository itemRepository, IGalleryCategoryRepository categoryRepository)
    : IRequestHandler<GetAllGalleryItemsQuery, OperationDataResult<List<GalleryItemResponse>>>
{
    public async Task<OperationDataResult<List<GalleryItemResponse>>> Handle(GetAllGalleryItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await itemRepository.QueryWithCategories().AsNoTracking().OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).ToListAsync(cancellationToken);
        var categories = await categoryRepository.GetAllAsync(cancellationToken: cancellationToken);

        var categoriesById = categories.ToDictionary(c => c.Id);

        var response = items
            .Select(item =>
            {
                var itemCategories = item.CategoryIds
                    .Where(categoriesById.ContainsKey)
                    .Select(id => GalleryCategoryResponse.FromDomain(categoriesById[id]))
                    .ToList();

                return GalleryItemResponse.FromDomain(item, itemCategories);
            })
            .ToList();

        return Result.Success(response, "Gallery items retrieved successfully.");
    }
}
