using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.ListTestimonials;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Testimonials;

public class ListTestimonials : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/testimonials/query", Handle)
            .WithTags("SiteContent/Testimonials")
            .WithName("ListTestimonials")
            .WithDescription("Returns testimonials, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new ListTestimonialsQuery(query.ToPageRequest(), dynamicQuery));
        return result.ToEnvelopedResult(httpContext);
    }
}
