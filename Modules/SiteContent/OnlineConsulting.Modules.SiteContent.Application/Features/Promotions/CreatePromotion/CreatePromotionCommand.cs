using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.CreatePromotion;

public record CreatePromotionCommand(string Title, string Description, string? CtaText, string? CtaUrl, DateTimeOffset? ExpiresAt, int DisplayOrder = 0)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreatePromotionHandler(IPromotionRepository repository) : IRequestHandler<CreatePromotionCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
    {
        var entity = new Promotion
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CtaText = request.CtaText,
            CtaUrl = request.CtaUrl,
            ExpiresAt = request.ExpiresAt,
            DisplayOrder = request.DisplayOrder,
        };

        _ = await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Promotion created successfully.");
    }
}
