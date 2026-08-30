using Hateoas;
using Hateoas.AspNetCore;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.GetSubscribers;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Newsletter;

public class GetSubscribers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/inquiries/newsletter", Handle)
            .WithTags("Inquiries/Newsletter")
            .RequireAuthorization()
            .WithName("GetSubscribers")
            .WithDescription("Returns newsletter subscribers, paginated. Admin only.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetSubscribersQuery(PageRequestFactory.Create(index, size)));
        return result
            .OnSuccess(page =>
            {
                foreach (var subscriber in page.Items)
                {
                    subscriber.Links = BuildLinks(httpContext, linkGenerator, subscriber.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }

    internal static Dictionary<string, Link> BuildLinks(HttpContext httpContext, LinkGenerator linkGenerator, Guid id)
        => httpContext.Links(linkGenerator)
            .AddCustom("delete", "Unsubscribe", "DELETE", new { id })
            .Build();
}
