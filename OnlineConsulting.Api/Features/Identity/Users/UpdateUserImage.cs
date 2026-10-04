using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUserImage;
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
            .DisableAntiforgery();
    }

    private static async Task<IResult> Handle(IFormFile image, ISender sender, HttpContext httpContext)
    {
        await using var content = image.OpenReadStream();

        return (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new UpdateUserImageCommand(user.Id, content, image.FileName, image.ContentType, image.Length))))
            .ToEnvelopedResult(httpContext);
    }
}
