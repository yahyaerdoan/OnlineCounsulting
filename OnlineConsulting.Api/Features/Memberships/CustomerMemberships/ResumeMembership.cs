using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ResumeMembership;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class ResumeMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/memberships/resume", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("ResumeMembership")
            .WithDescription("Resumes the current user's paused membership - billing continues normally.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());

        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new ResumeMembershipCommand(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
