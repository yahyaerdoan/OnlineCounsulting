using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.CrossCuttingConcernLayer.Slugs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Service;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.CreateServiceArea;

/// <summary>Slug is derived from Name+State and de-duplicated against existing slugs (see UpdateServiceAreaCommand, which never changes it once set).</summary>
public record CreateServiceAreaCommand(string Name, string State, string? IntroText, int DisplayOrder = 0) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Add];
}

public class CreateServiceAreaHandler(IServiceAreaRepository repository, ICityGeocoder geocoder) : IRequestHandler<CreateServiceAreaCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateServiceAreaCommand request, CancellationToken cancellationToken)
    {
        var slug = await SlugGenerator.GenerateUniqueAsync($"{request.Name} {request.State}", async prefix => await repository.Query().Where(s => s.Slug.StartsWith(prefix)).Select(s => s.Slug).ToListAsync(cancellationToken));

        var entity = new ServiceArea
        {
            Name = request.Name,
            State = request.State,
            Slug = slug,
            IntroText = request.IntroText,
            DisplayOrder = request.DisplayOrder,
        };

        if (await geocoder.GeocodeAsync(request.Name, request.State, cancellationToken) is { } point)
        {
            entity.Latitude = point.Latitude;
            entity.Longitude = point.Longitude;
        }

        _ = await repository.AddAsync(entity, cancellationToken: cancellationToken);

        return Result.Created(entity.Id, "Service area created successfully.");
    }
}
