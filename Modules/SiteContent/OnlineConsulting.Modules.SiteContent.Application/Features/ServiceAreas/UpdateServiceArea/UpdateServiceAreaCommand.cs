using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.UpdateServiceArea;

/// <summary>Never touches Slug - the SEO URL stays stable once published, even if Name changes.</summary>
public record UpdateServiceAreaCommand(Guid Id, string Name, string State, string? IntroText, int DisplayOrder = 0)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdateServiceAreaHandler(IServiceAreaRepository repository, ICityGeocoder geocoder) : IRequestHandler<UpdateServiceAreaCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateServiceAreaCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("ServiceArea", request.Id);
        }

        var placeChanged = !string.Equals(entity.Name, request.Name, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(entity.State, request.State, StringComparison.OrdinalIgnoreCase);

        entity.Name = request.Name;
        entity.State = request.State;

        if ((placeChanged || entity.Latitude is null) && await geocoder.GeocodeAsync(request.Name, request.State, cancellationToken) is { } point)
        {
            entity.Latitude = point.Latitude;
            entity.Longitude = point.Longitude;
        }
        else if (placeChanged)
        {
            entity.Latitude = null;
            entity.Longitude = null;
        }
        entity.IntroText = request.IntroText;
        entity.DisplayOrder = request.DisplayOrder;

        _ = await repository.UpdateAsync(entity);

        return Result.Success("Service area updated successfully.");
    }
}
