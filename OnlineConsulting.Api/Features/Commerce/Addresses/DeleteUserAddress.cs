using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.DeleteUserAddress;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class DeleteUserAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/addresses/{id:guid}", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("DeleteUserAddress")
            .WithDescription("Deletes one of the current user's addresses.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new DeleteUserAddressCommand(id, currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
