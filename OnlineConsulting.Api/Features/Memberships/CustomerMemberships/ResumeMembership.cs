using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ResumeMembership;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class ResumeMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/memberships/resume", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("ResumeMembership")
            .WithDescription("Resumes the current user's paused membership - billing continues normally.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new ResumeMembershipCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
