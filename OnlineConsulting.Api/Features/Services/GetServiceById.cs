using Hateoas;
using Hateoas.AspNetCore;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.GetServiceById;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Services;

public class GetServiceById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/services/{id:guid}", Handle)
            .WithTags("Services")
            .WithName("GetServiceById")
            .WithDescription("Returns a single service by id. Public - no login required to browse the catalog.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetServiceByIdQuery(id));
        return result
            .OnSuccess(service => service.Links = BuildLinks(httpContext, linkGenerator, service.Id))
            .ToEnvelopedResult(httpContext);
    }

    internal static Dictionary<string, Link> BuildLinks(HttpContext httpContext, LinkGenerator linkGenerator, Guid id)
        => httpContext.Links(linkGenerator)
            .Add("self", "GetServiceById", "GET", new { id })
            .Add("edit", "UpdateService", "PUT", new { id })
            .AddCustom("delete", "DeleteService", "DELETE", new { id })
            .Build();
}
