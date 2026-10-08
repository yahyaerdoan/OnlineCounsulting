using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using OnlineConsulting.Modules.Services.Application.Features.Services.GetServicesByCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class GetServicesByCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/categories/{categoryId:guid}/services", Handle)
            .WithTags("Services")
            .WithName("GetServicesByCategory")
            .WithDescription("Returns a category's services, paginated. Public - no login required.")
            .ProducesEnveloped<Paginate<ServiceResponse>>();
    }

    private static async Task<IResult> Handle(Guid categoryId, ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetServicesByCategoryQuery(categoryId, PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
