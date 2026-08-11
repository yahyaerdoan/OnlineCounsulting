namespace OnlineConsulting.Api.Common;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoint<TEndpoint>(this WebApplication app) where TEndpoint : IEndpoint, new()
    {
        new TEndpoint().MapEndpoint(app);
        return app;
    }
}
