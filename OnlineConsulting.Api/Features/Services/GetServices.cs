using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.Contracts;
using OnlineConsulting.Modules.Services.Application.Features.Services.GetServices;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class GetServices : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/services", Handle)
            .WithTags("Services")
            .WithName("GetServices")
            .WithDescription("Returns the current tenant's services, paginated. Public - no login required.")
            .ProducesEnveloped<Paginate<ServiceResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetServicesQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
