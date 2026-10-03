using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.UpdatePendingOrderAddresses;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class UpdatePendingOrderAddresses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/orders/{id:guid}/addresses", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("UpdatePendingOrderAddresses")
            .WithDescription("Points the current user's own unpaid order at their current default shipping and billing addresses (going back from payment to change an address).");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new UpdatePendingOrderAddressesCommand(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
