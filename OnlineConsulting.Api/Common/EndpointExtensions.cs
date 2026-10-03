using Hateoas.AspNetCore;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Common;

public static class EndpointExtensions
{
    /// <summary>Auto-registers every <see cref="IEndpoint"/> in this assembly, documenting their Problem Details responses and adding hypermedia (link providers, Location, paging Link header); skips <see cref="IDevOnlyEndpoint"/> outside Development.</summary>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup(string.Empty).ProducesResultProblems().WithHateoas();
        var endpointTypes = typeof(IEndpoint).Assembly.GetTypes().Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IEndpoint).IsAssignableFrom(type));

        foreach (var endpointType in endpointTypes)
        {
            var endpoint = Activator.CreateInstance(endpointType) as IEndpoint ?? throw new InvalidOperationException($"{endpointType.Name} could not be instantiated as an IEndpoint.");

            if (endpoint is IDevOnlyEndpoint && !app.Environment.IsDevelopment())
            {
                continue;
            }

            endpoint.MapEndpoint(endpoints);
        }

        return app;
    }
}
