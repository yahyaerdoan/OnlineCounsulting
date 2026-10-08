using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;

namespace OnlineConsulting.Api.Common;

public static class ApiJsonSetup
{
    /// <summary>Everything that shapes the JSON of API responses (HAL links included); the v1 golden contract tests run on the same setup.</summary>
    public static IServiceCollection AddApiJson(this IServiceCollection services)
    {
        _ = services.AddHateoas(options => options.CurieName = Rels.CurieName).AddLinkProvidersFromAssembly(typeof(ApiJsonSetup).Assembly);
        return services;
    }
}
