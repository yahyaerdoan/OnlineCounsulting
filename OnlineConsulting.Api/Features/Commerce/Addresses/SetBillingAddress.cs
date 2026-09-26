using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetBillingAddress;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class SetBillingAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/addresses/{id:guid}/billing", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("SetBillingAddress")
            .WithDescription("Marks one of the current user's addresses as the billing address.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new SetBillingAddressCommand(user.Id, id))))
            .ToEnvelopedResult(httpContext);
}
