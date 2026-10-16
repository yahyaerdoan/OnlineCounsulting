using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ListMyMessages;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class ListMyMessages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/inquiries/messages/mine", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("ListMyMessages")
            .WithDescription("The current user's conversations, newest activity first.")
            .ProducesEnveloped<List<MessageResponse>>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new ListMyMessagesQuery(currentUser.RequiredId()))).ToEnvelopedResult(httpContext);
}
