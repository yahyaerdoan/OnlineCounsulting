using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.UpdatePartnershipSocialLink;

public record UpdatePartnershipSocialLinkCommand(Guid Id, string Name, string Url, string Icon, string? IconColor = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdatePartnershipSocialLinkHandler(IPartnershipSocialLinkRepository repository) : IRequestHandler<UpdatePartnershipSocialLinkCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdatePartnershipSocialLinkCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Partnership social link", request.Id);
        }

        entity.Name = request.Name;
        entity.Url = request.Url;
        entity.Icon = request.Icon;
        entity.IconColor = request.IconColor;

        _ = await repository.UpdateAsync(entity);

        return Result.Success("Partnership social link updated successfully.");
    }
}
