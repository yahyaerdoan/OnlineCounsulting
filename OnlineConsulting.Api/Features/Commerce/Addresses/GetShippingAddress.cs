using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetShippingAddress;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class GetShippingAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/addresses/shipping", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("GetShippingAddress")
            .WithDescription("Returns the current user's shipping address.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetShippingAddressQuery(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}
