using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.UpdateFooterInfo;

public record UpdateFooterInfoCommand(Guid Id, string ImageUrl, string Description, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdateFooterInfoHandler(IFooterInfoRepository repository, IStorageService storageService) : IRequestHandler<UpdateFooterInfoCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateFooterInfoCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Footer info", request.Id);
        }

        entity.ImageUrl = storageService.ToStoredUrl(request.ImageUrl);
        entity.Description = request.Description;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        _ = await repository.UpdateAsync(entity);

        return Result.Success("Footer info updated successfully.");
    }
}
