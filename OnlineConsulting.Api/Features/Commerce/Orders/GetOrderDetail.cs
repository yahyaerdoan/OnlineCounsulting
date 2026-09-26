using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.GetOrderDetail;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class GetOrderDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/orders/{id:guid}", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("GetOrderDetail")
            .WithDescription("Returns a single order belonging to the current user, with its items and address ids.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetOrderDetailQuery(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
