using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Subscribe;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Newsletter;

public class SubscribeNewsletter : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/inquiries/newsletter", Handle)
            .WithTags("Inquiries/Newsletter")
            .WithName("SubscribeNewsletter")
            .WithDescription("Subscribes an email address to the newsletter. Public - no login required.")
            .ProducesEnveloped()
            .ProducesEnveloped(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] SubscribeNewsletterRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record SubscribeNewsletterRequest(string Email)
{
    public SubscribeNewsletterCommand ToCommand() => new(Email);
}
