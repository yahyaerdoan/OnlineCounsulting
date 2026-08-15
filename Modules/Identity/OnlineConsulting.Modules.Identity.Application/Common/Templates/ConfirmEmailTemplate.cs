using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Identity.Application.Common.Templates;

public record ConfirmEmailEmailModel(string FirstName, string ConfirmationUrl);

/// <summary>Email confirmation link sent right after registration.</summary>
public class ConfirmEmailTemplate : IEmailTemplate<ConfirmEmailEmailModel>
{
    public string Subject(ConfirmEmailEmailModel model) => "Confirm your email address";

    public string Build(ConfirmEmailEmailModel model) => EmailLayout.Wrap($"""
        <p>Hi {WebUtility.HtmlEncode(model.FirstName)},</p>
        <p>Thanks for signing up - please confirm your email address to activate your account.</p>
        <p><a href="{WebUtility.HtmlEncode(model.ConfirmationUrl)}" style="color: #4CAF50;">Confirm my email</a></p>
        <p>If you didn't create this account, you can ignore this email.</p>
        """);
}
