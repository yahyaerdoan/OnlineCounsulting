using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetBillingAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class SetBillingAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/addresses/{id:guid}/billing", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("SetBillingAddress")
            .WithDescription("Marks one of the current user's addresses as the billing address.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new SetBillingAddressCommand(currentUser.Data.Id, id));
        return result.ToEnvelopedResult(httpContext);
    }
}
