using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.UpdateFaqItem;

public record UpdateFaqItemCommand(Guid Id, Guid ServiceId, string Question, string Answer, int DisplayOrder = 0)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdateFaqItemHandler(IFaqItemRepository repository) : IRequestHandler<UpdateFaqItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateFaqItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("FaqItem", request.Id);
        }

        entity.ServiceId = request.ServiceId;
        entity.Question = request.Question;
        entity.Answer = request.Answer;
        entity.DisplayOrder = request.DisplayOrder;

        _ = await repository.UpdateAsync(entity);

        return Result.Success("FAQ item updated successfully.");
    }
}
