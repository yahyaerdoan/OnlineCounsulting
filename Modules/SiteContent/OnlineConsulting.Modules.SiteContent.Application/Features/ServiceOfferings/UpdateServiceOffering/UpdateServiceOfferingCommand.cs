using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.UpdateServiceOffering;

public record UpdateServiceOfferingCommand(Guid Id, string Title, string Description, string Icon, string? IconColor = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdateServiceOfferingHandler(IServiceOfferingRepository repository) : IRequestHandler<UpdateServiceOfferingCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateServiceOfferingCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Service offering", request.Id);

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.Icon = request.Icon;
        entity.IconColor = request.IconColor;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        await repository.UpdateAsync(entity);

        return Result.Success("Service offering updated successfully.");
    }
}
