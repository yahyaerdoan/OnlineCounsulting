using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.UpdatePendingOrderAddresses;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Orders;

public class UpdatePendingOrderAddresses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/orders/{id:guid}/addresses", Handle)
            .WithTags("Commerce/Orders")
            .RequireAuthorization()
            .WithName("UpdatePendingOrderAddresses")
            .WithDescription("Points the current user's own unpaid order at their current default shipping and billing addresses (going back from payment to change an address).")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new UpdatePendingOrderAddressesCommand(id, currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
