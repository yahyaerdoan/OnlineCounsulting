using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Identity.Application.Common.Templates;

public record PolicyNoticeEmailModel(string FirstName, string PrivacyPolicyUrl, string TermsOfServiceUrl);

/// <summary>Privacy policy / terms notice sent alongside the welcome email.</summary>
public class PolicyNoticeTemplate : IEmailTemplate<PolicyNoticeEmailModel>
{
    public string Subject(PolicyNoticeEmailModel model) => "Our Privacy Policy & Terms of Service";

    public string Build(PolicyNoticeEmailModel model) => EmailLayout.Wrap($"""
        <p>Hi {WebUtility.HtmlEncode(model.FirstName)},</p>
        <p>Before you get started, please take a moment to review our policies:</p>
        <p><a href="{WebUtility.HtmlEncode(model.PrivacyPolicyUrl)}" style="color: #4CAF50;">Privacy Policy</a></p>
        <p><a href="{WebUtility.HtmlEncode(model.TermsOfServiceUrl)}" style="color: #4CAF50;">Terms of Service</a></p>
        """);
}
