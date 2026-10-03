using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.CancelInvite;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Contracts;
using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Api.Features.Identity.Invites;

public sealed class InviteLinks : LinkProvider<InviteResponse>
{
    protected override void AddLinks(InviteResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(resource.Status == InviteStatuses.Pending && links.User.CanSend<CancelInviteCommand>(), Rels.Cancel, "CancelInvite", HttpMethods.Delete, new { id = resource.Id });
}
