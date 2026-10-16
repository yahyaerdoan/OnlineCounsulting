using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SendMyMessage;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class SendMyMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/inquiries/messages/mine", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("SendMyMessage")
            .WithDescription("Starts a conversation from the current user's inbox; name and email come from the account.")
            .ProducesEnveloped<MessageResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] SendMyMessageBody body, ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new SendMyMessageCommand(currentUser.RequiredId(), body.Subject, body.Description))).ToEnvelopedResult(httpContext);

    /// <summary>Wire-shape only - the user comes from the token.</summary>
    private sealed record SendMyMessageBody(string Subject, string Description);
}
