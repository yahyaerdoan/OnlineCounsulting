using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.DeleteFooterInfo;

public record DeleteFooterInfoCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Delete];
}

public class DeleteFooterInfoHandler(IFooterInfoRepository repository) : IRequestHandler<DeleteFooterInfoCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteFooterInfoCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Footer info", request.Id);
        }

        _ = await repository.DeleteAsync(entity);

        return Result.Success("Footer info deleted successfully.");
    }
}
