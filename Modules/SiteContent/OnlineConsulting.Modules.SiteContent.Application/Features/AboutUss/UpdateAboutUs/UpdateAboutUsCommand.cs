using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.UpdateAboutUs;

public record UpdateAboutUsCommand(Guid Id, string Title, string Description, string? CoverImage, string? VideoUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdateAboutUsHandler(IAboutUsRepository repository) : IRequestHandler<UpdateAboutUsCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateAboutUsCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("About Us", request.Id);
        }

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.CoverImage = request.CoverImage;
        entity.VideoUrl = request.VideoUrl;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        _ = await repository.UpdateAsync(entity);

        return Result.Success("About Us content updated successfully.");
    }
}
