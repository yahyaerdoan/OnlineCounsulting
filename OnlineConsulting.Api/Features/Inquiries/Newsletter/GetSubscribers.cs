using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.GetSubscribers;
using ResultHandler.AspNetCore.Extensions;

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

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetSubscribersQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
