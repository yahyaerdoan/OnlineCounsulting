using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Partnerships;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Functional;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PartnershipSocialLinks.CreatePartnershipSocialLink;

public record CreatePartnershipSocialLinkCommand(Guid PartnershipId, string Name, string Url, string Icon, string? IconColor = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreatePartnershipSocialLinkHandler(IPartnershipSocialLinkRepository repository, IPartnershipRepository partnershipRepository)
    : IRequestHandler<CreatePartnershipSocialLinkCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreatePartnershipSocialLinkCommand request, CancellationToken cancellationToken)
    {
        var partnershipExists = await partnershipRepository.AnyAsync(x => x.Id == request.PartnershipId, cancellationToken: cancellationToken);
        if (!partnershipExists)
            return SiteContentBusinessRules.NotFound("Partnership", request.PartnershipId).ToErrorDataResult<Guid>();

        var entity = new PartnershipSocialLink
        {
            Id = Guid.NewGuid(),
            PartnershipId = request.PartnershipId,
            Name = request.Name,
            Url = request.Url,
            Icon = request.Icon,
            IconColor = request.IconColor,
        };

        await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Partnership social link created successfully.");
    }
}
