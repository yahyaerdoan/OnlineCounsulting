using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.CreatePartnership;

public record CreatePartnershipCommand(
    string FirstName, string LastName, string Email, string Title, string CompanyName, string Description, string WebsiteUrl,
    Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreatePartnershipHandler(IPartnershipRepository repository) : IRequestHandler<CreatePartnershipCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreatePartnershipCommand request, CancellationToken cancellationToken)
    {
        var entity = new Partnership
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Title = request.Title,
            CompanyName = request.CompanyName,
            Description = request.Description,
            WebsiteUrl = request.WebsiteUrl,
            PhotoMediaAssetId = request.PhotoMediaAssetId,
            DisplayOrder = request.DisplayOrder,
            Metadata = MetadataSerializer.Serialize(request.Metadata),
        };

        await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Partnership created successfully.");
    }
}
