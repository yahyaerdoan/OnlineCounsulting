using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ListMessages;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class ListMessages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/inquiries/messages/query", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("ListMessages")
            .WithDescription("Returns submitted contact-form messages, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body. Admin only.")
            .ProducesEnveloped<Paginate<MessageResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
    {
        var result = await sender.Send(new ListMessagesQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()));
        return result.ToEnvelopedResult(httpContext);
    }
}
