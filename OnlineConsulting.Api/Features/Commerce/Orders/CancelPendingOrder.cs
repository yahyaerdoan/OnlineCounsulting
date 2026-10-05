using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.CancelPendingOrder;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class CancelPendingOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/orders/{id:guid}/cancel", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("CancelPendingOrder")
            .WithDescription("Cancels the current user's own still-unpaid order and restores its items to their basket.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new CancelPendingOrderCommand(id, currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
