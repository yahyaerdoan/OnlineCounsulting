using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Unsubscribe;

namespace OnlineConsulting.Api.Features.Inquiries.Newsletter;

public sealed class NewsletterSubscriberLinks : LinkProvider<NewsletterSubscriberResponse>
{
    protected override void AddLinks(NewsletterSubscriberResponse resource, HateoasLinkBuilder links)
        => links.AddCustomIf(links.User.CanSend<UnsubscribeCommand>(), Rels.Delete, "Unsubscribe", HttpMethods.Delete, new { id = resource.Id });
}
