using Hateoas;
using Hateoas.AspNetCore;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.GetMessages;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class GetMessages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/inquiries/messages", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("GetMessages")
            .WithDescription("Returns submitted contact-form messages, paginated. Admin only.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetMessagesQuery(PageRequestFactory.Create(index, size)));
        return result
            .OnSuccess(page =>
            {
                foreach (var message in page.Items)
                {
                    message.Links = BuildLinks(httpContext, linkGenerator, message.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }

    internal static Dictionary<string, Link> BuildLinks(HttpContext httpContext, LinkGenerator linkGenerator, Guid id)
        => httpContext.Links(linkGenerator)
            .AddCustom("delete", "DeleteMessage", "DELETE", new { id })
            .AddCustom("reply", "ReplyToMessage", "POST", new { id })
            .Build();
}
