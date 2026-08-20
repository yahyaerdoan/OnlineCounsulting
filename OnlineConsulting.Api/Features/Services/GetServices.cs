using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.GetServices;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Services;

public class GetServices : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/services", Handle)
            .WithTags("Services")
            .WithName("GetServices")
            .WithDescription("Returns the current tenant's services, paginated. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetServicesQuery(PageRequestFactory.Create(index, size)));
        return result
            .OnSuccess(page =>
            {
                foreach (var service in page.Items)
                {
                    service.Links = GetServiceById.BuildLinks(httpContext, linkGenerator, service.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
