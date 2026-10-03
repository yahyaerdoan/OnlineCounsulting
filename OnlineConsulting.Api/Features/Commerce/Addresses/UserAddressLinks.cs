using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

/// <summary>Address endpoints only ever return the caller's own addresses. There is no single-address GET route, so no "self".</summary>
public sealed class UserAddressLinks : LinkProvider<UserAddressResponse>
{
    protected override void AddLinks(UserAddressResponse resource, HateoasLinkBuilder links)
        => links
            .Add(LinkRelations.Edit, "UpdateUserAddress", HttpMethods.Put, new { id = resource.Id })
            .AddCustom(Rels.Delete, "DeleteUserAddress", HttpMethods.Delete, new { id = resource.Id })
            .AddCustomIf(!resource.IsShippingAddress, Rels.SetShipping, "SetShippingAddress", HttpMethods.Put, new { id = resource.Id })
            .AddCustomIf(!resource.IsBillingAddress, Rels.SetBilling, "SetBillingAddress", HttpMethods.Put, new { id = resource.Id });
}
