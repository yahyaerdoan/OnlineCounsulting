using Hateoas;
using Hateoas.AspNetCore;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

// Shared by every Addresses endpoint that returns a UserAddressResponse - there's no single
// GetAddressById route (only the list endpoint), so there's no "self" link to add here.
internal static class AddressLinks
{
    public static Dictionary<string, Link> Build(HttpContext httpContext, LinkGenerator linkGenerator, Guid id)
        => httpContext.Links(linkGenerator)
            .Add("edit", "UpdateUserAddress", "PUT", new { id })
            .AddCustom("delete", "DeleteUserAddress", "DELETE", new { id })
            .AddCustom("set-shipping", "SetShippingAddress", "PUT", new { id })
            .AddCustom("set-billing", "SetBillingAddress", "PUT", new { id })
            .Build();
}
