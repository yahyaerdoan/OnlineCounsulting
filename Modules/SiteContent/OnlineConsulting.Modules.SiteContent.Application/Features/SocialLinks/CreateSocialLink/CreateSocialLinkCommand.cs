using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.CreateSocialLink;

public record CreateSocialLinkCommand(string Name, string Url, string Icon, string? IconColor = null, int DisplayOrder = 0)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateSocialLinkHandler(ISocialLinkRepository repository) : IRequestHandler<CreateSocialLinkCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateSocialLinkCommand request, CancellationToken cancellationToken)
    {
        var entity = new SocialLink
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Url = request.Url,
            Icon = request.Icon,
            IconColor = request.IconColor,
            DisplayOrder = request.DisplayOrder,
        };

        _ = await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Social link created successfully.");
    }
}
