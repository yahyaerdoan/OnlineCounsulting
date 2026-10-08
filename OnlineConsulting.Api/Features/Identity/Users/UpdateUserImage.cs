using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUserImage;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class UpdateUserImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/users/me/image", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("UpdateUserImage")
            .WithDescription("Updates the current user's profile image.")
            .DisableAntiforgery()
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, IFormFile image, ISender sender, HttpContext httpContext)
    {
        await using var content = image.OpenReadStream();

        return (await sender.Send(new UpdateUserImageCommand(currentUser.RequiredId(), content, image.FileName, image.ContentType, image.Length)))
            .ToEnvelopedResult(httpContext);
    }
}
