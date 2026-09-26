using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.CreateFooterInfo;

public record CreateFooterInfoCommand(string ImageUrl, string Description, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Add];
}

public class CreateFooterInfoHandler(IFooterInfoRepository repository, IStorageService storageService) : IRequestHandler<CreateFooterInfoCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateFooterInfoCommand request, CancellationToken cancellationToken)
    {
        var entity = new FooterInfo { ImageUrl = storageService.ToStoredUrl(request.ImageUrl), Description = request.Description, DisplayOrder = request.DisplayOrder, Metadata = MetadataSerializer.Serialize(request.Metadata) };

        _ = await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Footer info created successfully.");
    }
}
