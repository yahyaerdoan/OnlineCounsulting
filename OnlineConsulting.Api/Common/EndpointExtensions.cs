namespace OnlineConsulting.Api.Common;

public static class EndpointExtensions
{
    /// <summary>Maps every <see cref="IEndpoint"/> found in this assembly - new endpoints register
    /// themselves just by existing, so Program.cs never needs a per-endpoint line.</summary>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointTypes = typeof(IEndpoint).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IEndpoint).IsAssignableFrom(type));

        foreach (var endpointType in endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(endpointType)!;
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
