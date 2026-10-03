using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;

/// <summary>Finds a US city's center for the service-area map. Returns null when it can't (unknown place, provider down) - the area just has no pin.</summary>
public interface ICityGeocoder
{
    Task<GeoPoint?> GeocodeAsync(string city, string state, CancellationToken cancellationToken = default);
}
