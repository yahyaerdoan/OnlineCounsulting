using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.GetMyMembership;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class GetMyMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/memberships/mine", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("GetMyMembership")
            .WithDescription("Returns the current user's active membership, if any.")
            .ProducesEnveloped<CustomerMembershipResponse>();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetMyMembershipQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
