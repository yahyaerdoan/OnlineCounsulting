using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using OnlineConsulting.Modules.SiteContent.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.DeleteHeroSlide;

public record DeleteHeroSlideCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class DeleteHeroSlideHandler(IHeroSlideRepository repository) : IRequestHandler<DeleteHeroSlideCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteHeroSlideCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Hero slide", request.Id);

        await repository.DeleteAsync(entity);

        return Result.Success("Hero slide deleted successfully.");
    }
}
