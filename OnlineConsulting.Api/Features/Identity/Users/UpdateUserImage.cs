using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUserImage;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class UpdateUserImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/users/me/image", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("UpdateUserImage")
            .WithDescription("Updates the current user's profile image.")
            .DisableAntiforgery();
    }

    private static async Task<IResult> Handle(IFormFile image, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new UpdateUserImageCommand(currentUser.Data.Id, image));
        return result.ToEnvelopedResult(httpContext);
    }
}
