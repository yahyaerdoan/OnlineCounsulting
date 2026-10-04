using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.ListGalleryCategories;

public record ListGalleryCategoriesQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null) : IRequest<OperationDataResult<Paginate<GalleryCategoryResponse>>>, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(GalleryCategory.Name), nameof(GalleryCategory.Description)]);
}

public class ListGalleryCategoriesHandler(IGalleryCategoryRepository repository)
    : IRequestHandler<ListGalleryCategoriesQuery, OperationDataResult<Paginate<GalleryCategoryResponse>>>
{
    public async Task<OperationDataResult<Paginate<GalleryCategoryResponse>>> Handle(ListGalleryCategoriesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.Query().ToDynamicPaginateAsync(request, defaultOrderBy: x => x.Name, tieBreaker: x => x.Id, cancellationToken: cancellationToken);

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
