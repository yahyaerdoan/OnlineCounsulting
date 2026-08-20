using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.CreateTestimonial;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Testimonials;

public class CreateTestimonial : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/site-content/testimonials", Handle)
            .WithTags("SiteContent/Testimonials")
            .RequireAuthorization()
            .WithName("CreateTestimonial")
            .WithDescription("Creates a customer testimonial.");
    }

    private static async Task<IResult> Handle([FromBody] CreateTestimonialCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
