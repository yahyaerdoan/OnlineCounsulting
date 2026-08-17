using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.CreateServiceProcessStep;

public record CreateServiceProcessStepCommand(string Title, string Description, string Icon, string? IconColor = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateServiceProcessStepHandler(IServiceProcessStepRepository repository) : IRequestHandler<CreateServiceProcessStepCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateServiceProcessStepCommand request, CancellationToken cancellationToken)
    {
        var entity = new ServiceProcessStep
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

        return Result.Created(entity.Id, "Service process step created successfully.");
    }
}
