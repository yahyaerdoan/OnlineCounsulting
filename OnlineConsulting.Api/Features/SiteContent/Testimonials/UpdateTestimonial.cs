using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.UpdateTestimonial;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Testimonials;

public class UpdateTestimonial : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/testimonials/{id:guid}", Handle)
            .WithTags("SiteContent/Testimonials")
            .RequireAuthorization()
            .WithName("UpdateTestimonial")
            .WithDescription("Updates a customer testimonial.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateTestimonialCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
