using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using OnlineConsulting.Modules.SiteContent.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.DeletePartnership;

public record DeletePartnershipCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class DeletePartnershipHandler(IPartnershipRepository repository) : IRequestHandler<DeletePartnershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeletePartnershipCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Partnership", request.Id);

        await repository.DeleteAsync(entity);

        return Result.Success("Partnership deleted successfully.");
    }
}
