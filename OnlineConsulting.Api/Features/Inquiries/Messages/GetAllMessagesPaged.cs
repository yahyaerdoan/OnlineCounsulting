using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.GetAllMessagesPaged;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class GetAllMessagesPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/inquiries/messages/query", Handle)
            .WithTags("Inquiries/Messages")
            .RequireAuthorization()
            .WithName("GetAllMessagesPaged")
            .WithDescription("Returns submitted contact-form messages, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body. Admin only.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllMessagesPagedQuery(query.ToPageRequest(), dynamicQuery));
        return result
            .OnSuccess(page =>
            {
                foreach (var message in page.Items)
                {
                    message.Links = GetMessages.BuildLinks(httpContext, linkGenerator, message.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
