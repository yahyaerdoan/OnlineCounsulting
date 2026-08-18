using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.DeleteServiceArea;

public record DeleteServiceAreaCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class DeleteServiceAreaHandler(IServiceAreaRepository repository) : IRequestHandler<DeleteServiceAreaCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteServiceAreaCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("ServiceArea", request.Id);

        await repository.DeleteAsync(entity);

        return Result.Success("Service area deleted successfully.");
    }
}
