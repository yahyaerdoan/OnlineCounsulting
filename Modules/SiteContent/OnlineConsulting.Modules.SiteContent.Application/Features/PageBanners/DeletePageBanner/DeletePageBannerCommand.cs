using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using OnlineConsulting.Modules.SiteContent.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.DeletePageBanner;

public record DeletePageBannerCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class DeletePageBannerHandler(IPageBannerRepository repository) : IRequestHandler<DeletePageBannerCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeletePageBannerCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Page banner", request.Id);

        await repository.DeleteAsync(entity);

        return Result.Success("Page banner deleted successfully.");
    }
}
