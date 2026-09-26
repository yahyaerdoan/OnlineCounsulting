using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.DeleteUserAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class DeleteUserAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/addresses/{id:guid}", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("DeleteUserAddress")
            .WithDescription("Deletes one of the current user's addresses.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new DeleteUserAddressCommand(id, user.Id))))
            .ToEnvelopedResult(httpContext);
}
