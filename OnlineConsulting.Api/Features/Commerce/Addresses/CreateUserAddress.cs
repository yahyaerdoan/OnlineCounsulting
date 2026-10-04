using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.CreateUserAddress;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class CreateUserAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/addresses", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("CreateUserAddress")
            .WithDescription("Creates a new address for the current user.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] CreateUserAddressRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}

public record CreateUserAddressRequest(string AddressName, string? CompanyName, string Country, string AddressLine, string City, string State, string Zipcode, string? Notes, bool IsShippingAddress, bool IsBillingAddress)
{
    public CreateUserAddressCommand ToCommand(Guid userId) => new(userId, AddressName, CompanyName, Country, AddressLine, City, State, Zipcode, Notes, IsShippingAddress, IsBillingAddress);
}
