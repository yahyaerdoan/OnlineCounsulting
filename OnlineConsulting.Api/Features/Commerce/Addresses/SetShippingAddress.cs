using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetShippingAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
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
            .WithDescription("Marks one of the current user's addresses as the shipping address.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new SetShippingAddressCommand(user.Id, id))))
            .ToEnvelopedResult(httpContext);
}
