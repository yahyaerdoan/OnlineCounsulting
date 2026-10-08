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
        _ = app.MapPut("/site-content/testimonials/{id:guid}", Handle)
            .WithTags("SiteContent/Testimonials")
            .RequireAuthorization()
            .WithName("UpdateTestimonial")
            .WithDescription("Updates a customer testimonial.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateTestimonialRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateTestimonialRequest(string FirstName, string LastName, string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateTestimonialCommand ToCommand(Guid id) => new(id, FirstName, LastName, Title, Description, ImageUrl, DisplayOrder, Metadata);
}
