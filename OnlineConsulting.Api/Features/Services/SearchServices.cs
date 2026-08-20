using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.SearchServices;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

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

    private static async Task<IResult> Handle(string query, ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new SearchServicesQuery(query));
        return result
            .OnSuccess(items =>
            {
                foreach (var service in items)
                {
                    service.Links = GetServiceById.BuildLinks(httpContext, linkGenerator, service.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
