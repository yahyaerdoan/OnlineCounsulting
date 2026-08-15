using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SubmitMessage;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class SubmitMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/inquiries/messages", Handle)
            .WithTags("Inquiries/Messages")
            .WithName("SubmitMessage")
            .WithDescription("Submits a contact-form message. Public - no login required.");
    }

    private static async Task<IResult> Handle([FromBody] SubmitMessageCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
