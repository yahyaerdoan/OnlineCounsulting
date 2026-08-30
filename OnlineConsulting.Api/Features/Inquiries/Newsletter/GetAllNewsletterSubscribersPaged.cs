using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.GetAllNewsletterSubscribersPaged;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Newsletter;

public class GetAllNewsletterSubscribersPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/inquiries/newsletter/query", Handle)
            .WithTags("Inquiries/Newsletter")
            .RequireAuthorization()
            .WithName("GetAllNewsletterSubscribersPaged")
            .WithDescription("Returns newsletter subscribers, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body. Admin only.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllNewsletterSubscribersPagedQuery(query.ToPageRequest(), dynamicQuery));
        return result
            .OnSuccess(page =>
            {
                foreach (var subscriber in page.Items)
                {
                    subscriber.Links = GetSubscribers.BuildLinks(httpContext, linkGenerator, subscriber.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
