using Asp.Versioning;
using Asp.Versioning.Builder;
using Hateoas.AspNetCore;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Common;

public static class EndpointExtensions
{
    /// <summary>
    /// Auto-registers every <see cref="IEndpoint"/> in this assembly under /api/v{version} (<see cref="IVersionNeutralEndpoint"/>s keep their own address),
    /// documenting their Problem Details responses and adding hypermedia (link providers, Location, paging Link header); skips <see cref="IDevOnlyEndpoint"/> outside Development.
    /// </summary>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        ApiVersionSet versions = app.NewApiVersionSet().HasApiVersion(ApiVersions.V1).ReportApiVersions().Build();
        var versioned = app.MapGroup("/api/v{version:apiVersion}").WithApiVersionSet(versions).HasApiVersion(ApiVersions.V1).ProducesResultProblems().WithHateoas();
        var neutral = app.MapGroup(string.Empty).WithApiVersionSet(versions).IsApiVersionNeutral().ProducesResultProblems().WithHateoas();
        var endpointTypes = typeof(IEndpoint).Assembly.GetTypes().Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IEndpoint).IsAssignableFrom(type));

        foreach (var endpointType in endpointTypes)
        {
            var endpoint = Activator.CreateInstance(endpointType) as IEndpoint ?? throw new InvalidOperationException($"{endpointType.Name} could not be instantiated as an IEndpoint.");

            if (endpoint is IDevOnlyEndpoint && !app.Environment.IsDevelopment())
            {
                continue;
            }

            endpoint.MapEndpoint(endpoint is IVersionNeutralEndpoint ? neutral : versioned);
        }

        return app;
    }
}
