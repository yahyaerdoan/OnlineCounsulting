using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.CreatePageBanner;

public record CreatePageBannerCommand(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Add];
}

public class CreatePageBannerHandler(IPageBannerRepository repository, IStorageService storageService) : IRequestHandler<CreatePageBannerCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreatePageBannerCommand request, CancellationToken cancellationToken)
    {
        var entity = new PageBanner { Title = request.Title, Description = request.Description, ImageUrl = storageService.ToStoredUrl(request.ImageUrl), DisplayOrder = request.DisplayOrder, Metadata = MetadataSerializer.Serialize(request.Metadata) };

        _ = await repository.AddAsync(entity, cancellationToken: cancellationToken);

        return Result.Created(entity.Id, "Page banner created successfully.");
    }
}
