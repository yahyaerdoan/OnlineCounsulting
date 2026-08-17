using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.UpdatePartnership;

public record UpdatePartnershipCommand(
    Guid Id, string FirstName, string LastName, string Email, string Title, string CompanyName, string Description, string WebsiteUrl,
    Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdatePartnershipHandler(IPartnershipRepository repository) : IRequestHandler<UpdatePartnershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdatePartnershipCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Partnership", request.Id);

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Title = request.Title;
        entity.CompanyName = request.CompanyName;
        entity.Description = request.Description;
        entity.WebsiteUrl = request.WebsiteUrl;
        entity.PhotoMediaAssetId = request.PhotoMediaAssetId;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        await repository.UpdateAsync(entity);

        return Result.Success("Partnership updated successfully.");
    }
}
