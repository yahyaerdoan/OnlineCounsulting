using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.GetMessage;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class GetMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/inquiries/messages/{id:guid}", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("GetMessage")
            .WithDescription("One submitted message with its replies. Admin only.")
            .ProducesEnveloped<MessageResponse>();
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetMessageQuery(id))).ToEnvelopedResult(httpContext);
}
