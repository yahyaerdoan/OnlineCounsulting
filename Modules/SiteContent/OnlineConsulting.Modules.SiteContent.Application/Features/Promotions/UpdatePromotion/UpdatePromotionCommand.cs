using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.UpdatePromotion;

public record UpdatePromotionCommand(Guid Id, string Title, string Description, string? CtaText, string? CtaUrl, DateTimeOffset? ExpiresAt, int DisplayOrder = 0)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdatePromotionHandler(IPromotionRepository repository) : IRequestHandler<UpdatePromotionCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Promotion", request.Id);
        }

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.CtaText = request.CtaText;
        entity.CtaUrl = request.CtaUrl;
        entity.ExpiresAt = request.ExpiresAt;
        entity.DisplayOrder = request.DisplayOrder;

        _ = await repository.UpdateAsync(entity);

        return Result.Success("Promotion updated successfully.");
    }
}
