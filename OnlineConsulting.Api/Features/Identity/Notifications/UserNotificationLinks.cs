using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Identity.Application.Features.Notifications.Contracts;

namespace OnlineConsulting.Api.Features.Identity.Notifications;

public sealed class UserNotificationLinks : LinkProvider<UserNotificationResponse>
{
    protected override void AddLinks(UserNotificationResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(!resource.IsRead, Rels.MarkRead, "MarkNotificationRead", HttpMethods.Post, new { id = resource.Id });
}
