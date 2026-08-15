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
        app.MapPost("/api/inquiries/newsletter", Handle)
            .WithTags("Inquiries/Newsletter")
            .WithName("SubscribeNewsletter")
            .WithDescription("Subscribes an email address to the newsletter. Public - no login required.");
    }

    private static async Task<IResult> Handle([FromBody] SubscribeNewsletterCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
