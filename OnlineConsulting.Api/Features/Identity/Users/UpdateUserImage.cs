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
        _ = app.MapPost("/api/users/me/image", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("UpdateUserImage")
            .WithDescription("Updates the current user's profile image.")
            .DisableAntiforgery();
    }

    private static async Task<IResult> Handle(IFormFile image, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new UpdateUserImageCommand(user.Id, image))))
            .ToEnvelopedResult(httpContext);
}
