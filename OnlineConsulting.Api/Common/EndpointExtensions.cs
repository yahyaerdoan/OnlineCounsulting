namespace OnlineConsulting.Api.Common;

public static class EndpointExtensions
{
    /// <summary>Maps every <see cref="IEndpoint"/> found in this assembly - new endpoints register
    /// themselves just by existing, so Program.cs never needs a per-endpoint line.
    /// <see cref="IDevOnlyEndpoint"/> implementations are skipped outside Development so
    /// dev-only tooling (e.g. the generic register endpoint) can never be reached in Preprod/Production.</summary>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointTypes = typeof(IEndpoint).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IEndpoint).IsAssignableFrom(type));

        foreach (var endpointType in endpointTypes)
        {
            var endpoint = Activator.CreateInstance(endpointType) as IEndpoint
                ?? throw new InvalidOperationException($"{endpointType.Name} could not be instantiated as an IEndpoint.");

            if (endpoint is IDevOnlyEndpoint && !app.Environment.IsDevelopment())
            {
                continue;
            }

            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
