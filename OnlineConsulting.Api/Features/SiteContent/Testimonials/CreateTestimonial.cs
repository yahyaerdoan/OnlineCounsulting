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
        _ = app.MapPost("/site-content/testimonials", Handle)
            .WithTags("SiteContent/Testimonials")
            .RequireAuthorization()
            .WithName("CreateTestimonial")
            .WithDescription("Creates a customer testimonial.");
    }

    private static async Task<IResult> Handle([FromBody] CreateTestimonialRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateTestimonialRequest(string FirstName, string LastName, string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public CreateTestimonialCommand ToCommand() => new(FirstName, LastName, Title, Description, ImageUrl, DisplayOrder, Metadata);
}
