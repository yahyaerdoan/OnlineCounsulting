using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.GetAllTestimonials;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Testimonials;

public class GetAllTestimonials : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/testimonials", Handle)
            .WithTags("SiteContent/Testimonials")
            .WithName("GetAllTestimonials")
            .WithDescription("Returns the tenant's customer testimonials. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllTestimonialsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
