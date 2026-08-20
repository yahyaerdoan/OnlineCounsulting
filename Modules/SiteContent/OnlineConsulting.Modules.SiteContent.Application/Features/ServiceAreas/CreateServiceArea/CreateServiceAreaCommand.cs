using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Service;
using OnlineConsulting.SharedKernel.Slugs;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.CreateServiceArea;

public record CreateServiceAreaCommand(string Name, string State, string? IntroText, int DisplayOrder = 0)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateServiceAreaHandler(IServiceAreaRepository repository) : IRequestHandler<CreateServiceAreaCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateServiceAreaCommand request, CancellationToken cancellationToken)
    {
        var slug = await SlugGenerator.GenerateUniqueAsync($"{request.Name} {request.State}", candidate => repository.AnyAsync(s => s.Slug == candidate, cancellationToken: cancellationToken));

        var entity = new ServiceArea
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            State = request.State,
            Slug = slug,
            IntroText = request.IntroText,
            DisplayOrder = request.DisplayOrder,
        };

        _ = await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Service area created successfully.");
    }
}
