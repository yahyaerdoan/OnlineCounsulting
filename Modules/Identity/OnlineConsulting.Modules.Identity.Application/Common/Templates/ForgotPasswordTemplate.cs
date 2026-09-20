using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Identity.Application.Common.Templates;

public record ForgotPasswordEmailModel(string FirstName, string ResetUrl);

/// <summary>Password reset link sent from ForgotPasswordHandler.</summary>
public class ForgotPasswordTemplate : IEmailTemplate<ForgotPasswordEmailModel>
{
    public string Subject(ForgotPasswordEmailModel model) => "Reset your password";

    public string Build(ForgotPasswordEmailModel model) => EmailLayout.Wrap($"""
        <p>Hi {WebUtility.HtmlEncode(model.FirstName)},</p>
        <p>We received a request to reset your password. Click below to choose a new one.</p>
        <p><a href="{WebUtility.HtmlEncode(model.ResetUrl)}" style="color: #4CAF50;">Reset my password</a></p>
        <p>If you didn't request this, you can ignore this email - your password will stay the same.</p>
        """);
}
