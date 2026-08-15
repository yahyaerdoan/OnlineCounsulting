using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.CreateServiceOffering;

public record CreateServiceOfferingCommand(string Title, string Description, string Icon, string? IconColor = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateServiceOfferingHandler(IServiceOfferingRepository repository) : IRequestHandler<CreateServiceOfferingCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateServiceOfferingCommand request, CancellationToken cancellationToken)
    {
        var entity = new ServiceOffering
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Icon = request.Icon,
            IconColor = request.IconColor,
            DisplayOrder = request.DisplayOrder,
            Metadata = MetadataSerializer.Serialize(request.Metadata),
        };

        await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Service offering created successfully.");
    }
}
