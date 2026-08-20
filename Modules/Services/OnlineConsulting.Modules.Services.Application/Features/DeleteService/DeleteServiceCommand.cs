using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Constants;
using OnlineConsulting.Modules.Services.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Services.Application.Features.DeleteService;

public record DeleteServiceCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ServicesOperationClaims.Admin, ServicesOperationClaims.Write, ServicesOperationClaims.Delete];
}

public class DeleteServiceHandler(IServiceRepository repository) : IRequestHandler<DeleteServiceCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await repository.GetAsync(s => s.Id == request.Id, cancellationToken: cancellationToken);
        if (service is null)
        {
            return ServiceBusinessRules.ServiceNotFound(request.Id);
        }

        _ = await repository.DeleteAsync(service);

        return Result.Success("Service deleted successfully.");
    }
}
