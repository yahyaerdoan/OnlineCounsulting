using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetAddresses;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class GetAddresses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/addresses", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("GetAddresses")
            .WithDescription("Returns the current user's addresses.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new GetAddressesQuery(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
