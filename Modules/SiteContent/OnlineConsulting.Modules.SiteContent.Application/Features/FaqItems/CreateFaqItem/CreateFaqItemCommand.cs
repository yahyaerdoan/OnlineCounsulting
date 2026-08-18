using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.CreateFaqItem;

public record CreateFaqItemCommand(Guid ServiceId, string Question, string Answer, int DisplayOrder = 0)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateFaqItemHandler(IFaqItemRepository repository) : IRequestHandler<CreateFaqItemCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateFaqItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new FaqItem
        {
            Id = Guid.NewGuid(),
            ServiceId = request.ServiceId,
            Question = request.Question,
            Answer = request.Answer,
            DisplayOrder = request.DisplayOrder,
        };

        await repository.AddAsync(entity);

        return Result.Created(entity.Id, "FAQ item created successfully.");
    }
}
