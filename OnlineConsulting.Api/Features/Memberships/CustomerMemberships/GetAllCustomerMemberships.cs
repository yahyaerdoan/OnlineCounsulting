using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.GetAllCustomerMemberships;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class GetAllCustomerMemberships : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/memberships", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("GetAllCustomerMemberships")
            .WithDescription("Returns all customer memberships, paginated (admin).");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetAllCustomerMembershipsQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
