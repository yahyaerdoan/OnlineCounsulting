using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMessage;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class ReplyToMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/inquiries/messages/{id:guid}/reply", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("ReplyToMessage")
            .WithDescription("Sends an admin reply to a submitted contact-form message. Admin only.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] ReplyMessageBody body, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new ReplyToMessageCommand(id, body.ReplyBody));
        return result.ToEnvelopedResult(httpContext);
    }

    /// <summary>Wire-shape only - keeps the id-from-route binding out of the Application-layer command.</summary>
    private sealed record ReplyMessageBody(string ReplyBody);
}
