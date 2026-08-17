using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.UpdateSocialLink;

public record UpdateSocialLinkCommand(Guid Id, string Name, string Url, string Icon, string? IconColor = null, int DisplayOrder = 0)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdateSocialLinkHandler(ISocialLinkRepository repository) : IRequestHandler<UpdateSocialLinkCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateSocialLinkCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("SocialLink", request.Id);

        entity.Name = request.Name;
        entity.Url = request.Url;
        entity.Icon = request.Icon;
        entity.IconColor = request.IconColor;
        entity.DisplayOrder = request.DisplayOrder;

        await repository.UpdateAsync(entity);

        return Result.Success("Social link updated successfully.");
    }
}
