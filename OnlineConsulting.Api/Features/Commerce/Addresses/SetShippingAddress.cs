using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetShippingAddress;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class SetShippingAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/addresses/{id:guid}/shipping", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("SetShippingAddress")
            .WithDescription("Marks one of the current user's addresses as the shipping address.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new SetShippingAddressCommand(currentUser.RequiredId(), id)))
            .ToEnvelopedResult(httpContext);
}
