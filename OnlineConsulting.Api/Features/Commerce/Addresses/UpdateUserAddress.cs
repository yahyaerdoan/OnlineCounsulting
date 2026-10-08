using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.UpdateUserAddress;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class UpdateUserAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/addresses/{id:guid}", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("UpdateUserAddress")
            .WithDescription("Updates one of the current user's addresses.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, Guid id, [FromBody] UpdateUserAddressRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(id, currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}

public record UpdateUserAddressRequest(string AddressName, string? CompanyName, string Country, string AddressLine, string City, string State, string Zipcode, string? Notes, bool IsShippingAddress, bool IsBillingAddress)
{
    public UpdateUserAddressCommand ToCommand(Guid id, Guid userId) => new(id, userId, AddressName, CompanyName, Country, AddressLine, City, State, Zipcode, Notes, IsShippingAddress, IsBillingAddress);
}
