using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.DeleteMessage;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMessage;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

/// <summary>The sender replies through their own inbox; staff reply and delete when their permissions allow it.</summary>
public sealed class MessageLinks : LinkProvider<MessageResponse>
{
    protected override void AddLinks(MessageResponse resource, HateoasLinkBuilder links)
    {
        var id = new { id = resource.Id };
        var isOwner = resource.UserId is { } ownerId && links.User.IsUser(ownerId);

        _ = links
            .AddCustomIf(isOwner, Rels.Reply, "ReplyToMyMessage", HttpMethods.Post, id)
            .AddCustomIf(!isOwner && links.User.CanSend<ReplyToMessageCommand>(), Rels.Reply, "ReplyToMessage", HttpMethods.Post, id)
            .AddCustomIf(links.User.CanSend<DeleteMessageCommand>(), Rels.Delete, "DeleteMessage", HttpMethods.Delete, id);
    }
}
