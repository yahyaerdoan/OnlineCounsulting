using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.ChangePassword;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class ChangePassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/users/me/password", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("ChangePassword")
            .WithDescription("Changes the current user's password.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] ChangePasswordRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}

public record ChangePasswordRequest(string CurrentPassword, string NewPassword)
{
    public ChangePasswordCommand ToCommand(Guid userId) => new(userId, CurrentPassword, NewPassword);
}
