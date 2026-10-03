using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.SearchServices;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class SearchServices : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/services/search", Handle)
            .WithTags("Services")
            .WithName("SearchServices")
            .WithDescription("Searches services by title/description. Public - no login required.");
    }

    private static async Task<IResult> Handle(string query, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new SearchServicesQuery(query));
        return result.ToEnvelopedResult(httpContext);
    }
}
