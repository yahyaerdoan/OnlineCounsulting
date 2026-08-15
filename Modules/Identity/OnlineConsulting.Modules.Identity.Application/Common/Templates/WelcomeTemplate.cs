using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Identity.Application.Common.Templates;

public record WelcomeEmailModel(string FirstName, string LastName);

/// <summary>Welcome email sent once email confirmation succeeds.</summary>
public class WelcomeTemplate : IEmailTemplate<WelcomeEmailModel>
{
    public string Subject(WelcomeEmailModel model) => "Welcome to OnlineConsulting!";

    public string Build(WelcomeEmailModel model) => EmailLayout.Wrap($"""
        <p>Hi {WebUtility.HtmlEncode(model.FirstName)} {WebUtility.HtmlEncode(model.LastName)},</p>
        <p>Your email is confirmed and your account is now active. Welcome aboard!</p>
        """);
}
