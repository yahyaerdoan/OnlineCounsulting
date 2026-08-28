using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.DeleteFaqItem;

public record DeleteFaqItemCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Delete];
}

public class DeleteFaqItemHandler(IFaqItemRepository repository) : IRequestHandler<DeleteFaqItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteFaqItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("FaqItem", request.Id);
        }

        _ = await repository.DeleteAsync(entity);

        return Result.Success("FAQ item deleted successfully.");
    }
}
