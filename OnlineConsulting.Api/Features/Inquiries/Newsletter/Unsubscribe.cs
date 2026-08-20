using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Unsubscribe;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Newsletter;

public class Unsubscribe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/inquiries/newsletter/{id:guid}", Handle)
            .WithTags("Inquiries/Newsletter")
            .RequireAuthorization()
            .WithName("Unsubscribe")
            .WithDescription("Removes a newsletter subscriber. Admin only.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new UnsubscribeCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
