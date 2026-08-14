using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetShippingAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class GetShippingAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/addresses/shipping", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("GetShippingAddress")
            .WithDescription("Returns the current user's shipping address.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new GetShippingAddressQuery(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
