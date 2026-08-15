using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using OnlineConsulting.Modules.SiteContent.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.UpdatePageBanner;

public record UpdatePageBannerCommand(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdatePageBannerHandler(IPageBannerRepository repository) : IRequestHandler<UpdatePageBannerCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdatePageBannerCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Page banner", request.Id);

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ImageUrl = request.ImageUrl;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        await repository.UpdateAsync(entity);

        return Result.Success("Page banner updated successfully.");
    }
}
