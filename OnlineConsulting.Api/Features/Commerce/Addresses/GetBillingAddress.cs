using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetBillingAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class GetBillingAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/addresses/billing", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("GetBillingAddress")
            .WithDescription("Returns the current user's billing address.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new GetBillingAddressQuery(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
