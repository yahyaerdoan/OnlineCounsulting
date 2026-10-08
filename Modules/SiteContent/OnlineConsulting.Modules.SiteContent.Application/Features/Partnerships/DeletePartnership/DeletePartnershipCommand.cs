using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.DeletePartnership;

public record DeletePartnershipCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Delete];
}

public class DeletePartnershipHandler(IPartnershipRepository repository) : IRequestHandler<DeletePartnershipCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeletePartnershipCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Partnership", request.Id);
        }

        _ = await repository.DeleteAsync(entity, cancellationToken: cancellationToken);

        return Result.Success("Partnership deleted successfully.");
    }
}
