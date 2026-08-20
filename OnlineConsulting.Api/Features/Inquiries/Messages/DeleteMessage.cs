using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.DeleteMessage;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class DeleteMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/inquiries/messages/{id:guid}", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("DeleteMessage")
            .WithDescription("Deletes a submitted message. Admin only.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteMessageCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
