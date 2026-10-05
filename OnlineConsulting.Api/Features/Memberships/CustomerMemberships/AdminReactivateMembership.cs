using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.AdminReactivateMembership;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class AdminReactivateMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/memberships/{id:guid}/reactivate", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("AdminReactivateMembership")
            .WithDescription("Undoes a customer's pending cancellation before the period ends (admin).")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new AdminReactivateMembershipCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
