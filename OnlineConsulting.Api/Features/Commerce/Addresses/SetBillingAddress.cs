using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetBillingAddress;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class SetBillingAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/addresses/{id:guid}/billing", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("SetBillingAddress")
            .WithDescription("Marks one of the current user's addresses as the billing address.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new SetBillingAddressCommand(currentUser.RequiredId(), id)))
            .ToEnvelopedResult(httpContext);
}
