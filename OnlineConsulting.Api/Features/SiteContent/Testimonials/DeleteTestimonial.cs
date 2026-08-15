using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.DeleteTestimonial;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Testimonials;

public class DeleteTestimonial : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/site-content/testimonials/{id:guid}", Handle)
            .WithTags("SiteContent/Testimonials")
            .RequireAuthorization()
            .WithName("DeleteTestimonial")
            .WithDescription("Deletes a customer testimonial.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteTestimonialCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
