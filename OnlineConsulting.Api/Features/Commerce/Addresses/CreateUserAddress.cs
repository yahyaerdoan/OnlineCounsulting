using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.CreateUserAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class CreateUserAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/addresses", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("CreateUserAddress")
            .WithDescription("Creates a new address for the current user.");
    }

    private static async Task<IResult> Handle([FromBody] CreateUserAddressCommand command, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(command with { UserId = currentUser.Data.Id });
        return result.ToEnvelopedResult(httpContext);
    }
}
