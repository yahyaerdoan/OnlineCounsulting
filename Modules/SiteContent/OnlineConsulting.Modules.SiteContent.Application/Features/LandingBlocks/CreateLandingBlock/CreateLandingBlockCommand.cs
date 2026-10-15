using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Landing;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.CreateLandingBlock;

public record CreateLandingBlockCommand(string Section, string Title, string Body, string? Icon = null, int DisplayOrder = 0)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest, ILandingBlockFields
{
    public string[] Roles => LandingAccess.Roles;

    /// <summary>Platform-level; a tenant admin must never reach this.</summary>
    public bool AllowTenantBypass => false;
}

public class CreateLandingBlockHandler(ILandingBlockRepository repository) : IRequestHandler<CreateLandingBlockCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateLandingBlockCommand request, CancellationToken cancellationToken)
    {
        var entity = new LandingBlock
        {
            Section = request.Section,
            Title = request.Title,
            Body = request.Body,
            Icon = request.Icon,
            DisplayOrder = request.DisplayOrder,
        };

        _ = await repository.AddAsync(entity, cancellationToken: cancellationToken);

        return Result.Created(entity.Id, "Landing block created successfully.");
    }
}
