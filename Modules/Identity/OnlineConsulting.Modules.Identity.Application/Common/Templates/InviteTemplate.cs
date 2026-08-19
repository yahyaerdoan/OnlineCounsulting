using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Identity.Application.Common.Templates;

public record InviteEmailModel(string InviteUrl);

/// <summary>Invitation link sent to a teammate an existing tenant admin has invited. Clicking the link and
/// setting a password is all the invited person needs to do - the tenant is resolved from the token itself.</summary>
public class InviteTemplate : IEmailTemplate<InviteEmailModel>
{
    public string Subject(InviteEmailModel model) => "You've been invited to join OnlineConsulting";

    public string Build(InviteEmailModel model) => EmailLayout.Wrap($"""
        <p>Hi,</p>
        <p>You've been invited to join a team on OnlineConsulting. Click the link below to set your password and get started.</p>
        <p><a href="{WebUtility.HtmlEncode(model.InviteUrl)}" style="color: #4CAF50;">Accept invitation</a></p>
        <p>If you weren't expecting this invitation, you can ignore this email.</p>
        """);
}
