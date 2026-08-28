using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.CreatePageBanner;

public record CreatePageBannerCommand(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Add];
}

public class CreatePageBannerHandler(IPageBannerRepository repository) : IRequestHandler<CreatePageBannerCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreatePageBannerCommand request, CancellationToken cancellationToken)
    {
        var entity = new PageBanner { Id = Guid.NewGuid(), Title = request.Title, Description = request.Description, ImageUrl = request.ImageUrl, DisplayOrder = request.DisplayOrder, Metadata = MetadataSerializer.Serialize(request.Metadata) };

        _ = await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Page banner created successfully.");
    }
}
