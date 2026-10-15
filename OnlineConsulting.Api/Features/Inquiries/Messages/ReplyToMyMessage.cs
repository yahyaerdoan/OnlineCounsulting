using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMyMessage;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class ReplyToMyMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/inquiries/messages/mine/{id:guid}/reply", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("ReplyToMyMessage")
            .WithDescription("Adds the current user's reply to one of their conversations.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] ReplyMessageBody body, ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new ReplyToMyMessageCommand(currentUser.RequiredId(), id, body.ReplyBody))).ToEnvelopedResult(httpContext);

    /// <summary>Wire-shape only - keeps the id-from-route binding out of the Application-layer command.</summary>
    private sealed record ReplyMessageBody(string ReplyBody);
}
