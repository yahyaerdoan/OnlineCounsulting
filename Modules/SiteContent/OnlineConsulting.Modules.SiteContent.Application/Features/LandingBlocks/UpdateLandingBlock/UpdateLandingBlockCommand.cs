using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.UpdateLandingBlock;

public record UpdateLandingBlockCommand(Guid Id, string Section, string Title, string Body, string? Icon = null, int DisplayOrder = 0)
    : IRequest<OperationResult>, ISecureAddRequest, ILandingBlockFields
{
    public string[] Roles => LandingAccess.Roles;

    /// <summary>Platform-level; a tenant admin must never reach this.</summary>
    public bool AllowTenantBypass => false;
}

public class UpdateLandingBlockHandler(ILandingBlockRepository repository) : IRequestHandler<UpdateLandingBlockCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateLandingBlockCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Landing block", request.Id);
        }

        entity.Section = request.Section;
        entity.Title = request.Title;
        entity.Body = request.Body;
        entity.Icon = request.Icon;
        entity.DisplayOrder = request.DisplayOrder;

        _ = await repository.UpdateAsync(entity, cancellationToken: cancellationToken);

        return Result.Success("Landing block updated successfully.");
    }
}
