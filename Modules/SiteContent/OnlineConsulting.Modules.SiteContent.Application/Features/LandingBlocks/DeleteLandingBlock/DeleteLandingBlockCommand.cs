using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.DeleteLandingBlock;

public record DeleteLandingBlockCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => LandingAccess.Roles;

    /// <summary>Platform-level; a tenant admin must never reach this.</summary>
    public bool AllowTenantBypass => false;
}

public class DeleteLandingBlockHandler(ILandingBlockRepository repository) : IRequestHandler<DeleteLandingBlockCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteLandingBlockCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Landing block", request.Id);
        }

        _ = await repository.DeleteAsync(entity, cancellationToken: cancellationToken);

        return Result.Success("Landing block deleted successfully.");
    }
}
