using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.SetPromoCodeActive;

namespace OnlineConsulting.Api.Features.Memberships.PromoCodes;

public sealed class PromoCodeLinks : LinkProvider<PromoCodeResponse>
{
    protected override void AddLinks(PromoCodeResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(links.User.CanSend<SetPromoCodeActiveCommand>(), Rels.SetActive, "SetPromoCodeActive", HttpMethods.Put, new { id = resource.Id },
            resource.IsActive ? "Deactivate" : "Activate");
}
