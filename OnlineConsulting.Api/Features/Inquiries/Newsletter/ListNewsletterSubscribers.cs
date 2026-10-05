using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.ListNewsletterSubscribers;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Newsletter;

public class ListNewsletterSubscribers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/inquiries/newsletter/query", Handle)
            .WithTags("Inquiries/Newsletter")
            .RequireAuthorization()
            .WithName("ListNewsletterSubscribers")
            .WithDescription("Returns newsletter subscribers, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body. Admin only.")
            .ProducesEnveloped<Paginate<NewsletterSubscriberResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
    {
        var result = await sender.Send(new ListNewsletterSubscribersQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()));
        return result.ToEnvelopedResult(httpContext);
    }
}
