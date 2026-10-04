using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Hateoas;
using Hateoas.AspNetCore;

namespace OnlineConsulting.Api.Common.Hateoas;

/// <summary>
/// Links of a staff-managed content type that has no single-item GET route: "edit" and "oc:delete", each only for callers allowed to
/// send the matching command. Anonymous visitors of the public site therefore get no links. Override AddLinks (calling base) for extras.
/// </summary>
public abstract class ManagedContentLinks<TResource, TUpdate, TDelete>(string updateRouteName, string deleteRouteName, Func<TResource, Guid> idOf) : LinkProvider<TResource>
    where TResource : class
    where TUpdate : ISecureAddRequest
    where TDelete : ISecureAddRequest
{
    protected override void AddLinks(TResource resource, HateoasLinkBuilder links)
        => links
            .AddIf(links.User.CanSend<TUpdate>(), LinkRelations.Edit, updateRouteName, HttpMethods.Put, new { id = idOf(resource) })
            .AddCustomIf(links.User.CanSend<TDelete>(), Rels.Delete, deleteRouteName, HttpMethods.Delete, new { id = idOf(resource) });
}
