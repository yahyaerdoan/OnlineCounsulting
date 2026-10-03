using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.DeleteMessage;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMessage;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public sealed class MessageLinks : LinkProvider<MessageResponse>
{
    protected override void AddLinks(MessageResponse resource, HateoasLinkBuilder links)
        => links
            .AddCustomIf(links.User.CanSend<ReplyToMessageCommand>(), Rels.Reply, "ReplyToMessage", HttpMethods.Post, new { id = resource.Id })
            .AddCustomIf(links.User.CanSend<DeleteMessageCommand>(), Rels.Delete, "DeleteMessage", HttpMethods.Delete, new { id = resource.Id });
}
