using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.GetAllGalleryCategories;

/// <summary>Admin-only (legacy's dropdown-population equivalent) - unlike GalleryItem itself, the category list is an admin concern, not public content.</summary>
public record GetAllGalleryCategoriesQuery : IRequest<OperationDataResult<List<GalleryCategoryResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Read];
}

public class GetAllGalleryCategoriesHandler(IGalleryCategoryRepository repository)
    : IRequestHandler<GetAllGalleryCategoriesQuery, OperationDataResult<List<GalleryCategoryResponse>>>
{
    public async Task<OperationDataResult<List<GalleryCategoryResponse>>> Handle(GetAllGalleryCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetListAsync(orderBy: q => q.OrderBy(x => x.Name), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var response = categories.Items.Select(GalleryCategoryResponse.FromDomain).ToList();

        return Result.Success(response, "Gallery categories retrieved successfully.");
    }
}
