using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.ReorderLandingBlocks;

/// <summary>Sets one section's order in a single save: OrderedIds lists every entry of the section, first to last.</summary>
public record ReorderLandingBlocksCommand(string Section, IReadOnlyList<Guid> OrderedIds) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => LandingAccess.Roles;

    /// <summary>Platform-level; a tenant admin must never reach this.</summary>
    public bool AllowTenantBypass => false;
}

public class ReorderLandingBlocksHandler(ILandingBlockRepository repository) : IRequestHandler<ReorderLandingBlocksCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ReorderLandingBlocksCommand request, CancellationToken cancellationToken)
    {
        var blocks = await repository.GetAllAsync(b => b.Section == request.Section, cancellationToken: cancellationToken);

        // A stale list (an entry added or deleted meanwhile) would leave gaps or ties.
        if (blocks.Count != request.OrderedIds.Count || !blocks.All(b => request.OrderedIds.Contains(b.Id)))
        {
            return Result.Conflict(SiteContentMessages.LandingBlockOrderOutdated);
        }

        var positions = request.OrderedIds.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index + 1);
        var changed = blocks.Where(b => b.DisplayOrder != positions[b.Id]).ToList();

        foreach (var block in changed)
        {
            block.DisplayOrder = positions[block.Id];
        }

        if (changed.Count > 0)
        {
            _ = await repository.UpdateRangeAsync(changed, cancellationToken: cancellationToken);
        }

        return Result.Success(SiteContentMessages.LandingBlocksReordered);
    }
}
