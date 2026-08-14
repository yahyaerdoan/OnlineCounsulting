using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.GetServicesByCategory;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Services;

public class GetServicesByCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/{categoryId:guid}/services", Handle)
            .WithTags("Services")
            .WithName("GetServicesByCategory")
            .WithDescription("Returns a category's services, paginated. Public - no login required.");
    }

    private static async Task<IResult> Handle(Guid categoryId, ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetServicesByCategoryQuery(categoryId, PageRequestFactory.Create(index, size)));
        return result
            .OnSuccess(page =>
            {
                foreach (var service in page.Items)
                    service.Links = GetServiceById.BuildLinks(httpContext, linkGenerator, service.Id);
            })
            .ToEnvelopedResult(httpContext);
    }
}
