using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.UpdateUserAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class UpdateUserAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/addresses/{id:guid}", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("UpdateUserAddress")
            .WithDescription("Updates one of the current user's addresses.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateUserAddressCommand command, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(command with { Id = id, UserId = user.Id })))
            .ToEnvelopedResult(httpContext);
}
