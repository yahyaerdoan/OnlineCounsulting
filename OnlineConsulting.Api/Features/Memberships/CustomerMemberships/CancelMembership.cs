using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.CancelMembership;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class CancelMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/memberships/cancel", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("CancelMembership")
            .WithDescription("Cancels the current user's active membership at the end of the current billing period - access continues until then.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new CancelMembershipCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
