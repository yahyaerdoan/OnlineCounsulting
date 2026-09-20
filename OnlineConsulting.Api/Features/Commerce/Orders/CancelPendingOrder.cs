using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.CancelPendingOrder;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class CancelPendingOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/orders/{id:guid}/cancel", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("CancelPendingOrder")
            .WithDescription("Cancels the current user's own still-unpaid order and restores its items to their basket.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());

        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new CancelPendingOrderCommand(id, currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
