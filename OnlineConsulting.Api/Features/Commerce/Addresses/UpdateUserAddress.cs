using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.UpdateUserAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class UpdateUserAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/addresses/{id:guid}", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("UpdateUserAddress")
            .WithDescription("Updates one of the current user's addresses.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateUserAddressCommand command, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(command with { Id = id, UserId = currentUser.Data.Id });
        return result.ToEnvelopedResult(httpContext);
    }
}
