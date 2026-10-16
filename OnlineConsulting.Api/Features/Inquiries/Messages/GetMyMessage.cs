using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.GetMyMessage;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class GetMyMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/inquiries/messages/mine/{id:guid}", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("GetMyMessage")
            .WithDescription("One of the current user's conversations with its replies.")
            .ProducesEnveloped<MessageResponse>();
    }

    private static async Task<IResult> Handle(Guid id, ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetMyMessageQuery(currentUser.RequiredId(), id))).ToEnvelopedResult(httpContext);
}
