using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.DeletePartnershipSocialLink;

public record DeletePartnershipSocialLinkCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class DeletePartnershipSocialLinkHandler(IPartnershipSocialLinkRepository repository) : IRequestHandler<DeletePartnershipSocialLinkCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeletePartnershipSocialLinkCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Partnership social link", request.Id);

        await repository.DeleteAsync(entity);

        return Result.Success("Partnership social link deleted successfully.");
    }
}
