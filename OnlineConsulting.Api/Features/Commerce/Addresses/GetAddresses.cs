using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetAddresses;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class GetAddresses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/addresses", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("GetAddresses")
            .WithDescription("Returns the current user's addresses.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetAddressesQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
