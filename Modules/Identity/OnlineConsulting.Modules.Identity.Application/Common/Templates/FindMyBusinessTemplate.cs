using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Net;

namespace OnlineConsulting.Modules.Identity.Application.Common.Templates;

public record FindMyBusinessEmailModel(IReadOnlyList<BusinessSignInLink> Businesses);

/// <summary>Lists every business an email address has an account with, each with its own sign-in link.</summary>
public class FindMyBusinessTemplate : IEmailTemplate<FindMyBusinessEmailModel>
{
    public string Subject(FindMyBusinessEmailModel model) => "Your sign-in links";

    public string Build(FindMyBusinessEmailModel model) => EmailLayout.Wrap($"""
        <p>Hi,</p>
        <p>You asked where to sign in. Your email address has an account with:</p>
        <ul>
        {string.Concat(model.Businesses.Select(b => $"""<li><a href="{WebUtility.HtmlEncode(b.SignInUrl)}" style="color: #4CAF50;">{WebUtility.HtmlEncode(b.BusinessName)}</a></li>"""))}
        </ul>
        <p>If you didn't ask for this, you can ignore this email.</p>
        """);
}
