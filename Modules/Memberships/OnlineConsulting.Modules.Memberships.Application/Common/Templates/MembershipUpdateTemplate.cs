using System.Globalization;
using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Memberships.Application.Common.Templates;

public enum MembershipUpdateKind
{
    PaymentFailed,
    CancelledAfterFailures,
    CancelledByStaff,
    ReactivatedByStaff,
}

/// <summary>RenewsOn is the next renewal date, shown when the membership keeps running.</summary>
public record MembershipUpdateEmailModel(MembershipUpdateKind Kind, string? FirstName, string PlanName, DateTimeOffset? RenewsOn);

/// <summary>Membership changes the member did not make themselves (staff actions, failed payments).</summary>
public class MembershipUpdateTemplate : IEmailTemplate<MembershipUpdateEmailModel>
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public string Subject(MembershipUpdateEmailModel model) => model.Kind switch
    {
        MembershipUpdateKind.PaymentFailed => $"Action needed: your {model.PlanName} membership payment failed",
        MembershipUpdateKind.CancelledAfterFailures => $"Your {model.PlanName} membership was cancelled",
        MembershipUpdateKind.CancelledByStaff => $"Your {model.PlanName} membership was cancelled",
        _ => $"Your {model.PlanName} membership is active again",
    };

    public string Build(MembershipUpdateEmailModel model)
    {
        var greeting = string.IsNullOrWhiteSpace(model.FirstName) ? "Hi," : $"Hi {WebUtility.HtmlEncode(model.FirstName)},";
        var plan = $"<strong>{WebUtility.HtmlEncode(model.PlanName)}</strong>";
        var body = model.Kind switch
        {
            MembershipUpdateKind.PaymentFailed =>
                $"<p>We couldn't process the latest payment for your {plan} membership.</p><p>Please update your payment method soon so you keep your member discounts and priority visits.</p>",
            MembershipUpdateKind.CancelledAfterFailures =>
                $"<p>Your {plan} membership was cancelled after repeated payment failures, so it won't be charged again.</p><p>You can subscribe again anytime to get your benefits back.</p>",
            MembershipUpdateKind.CancelledByStaff =>
                $"<p>Our team cancelled your {plan} membership, so it won't be charged again.</p><p>If this is unexpected, just reply to this email or contact us and we'll sort it out.</p>",
            _ =>
                $"<p>Our team reactivated your {plan} membership. Nothing else changes: your benefits continue"
                + (model.RenewsOn is { } renewsOn ? $" and it renews on {renewsOn.ToString("MMMM d, yyyy", Usd)}." : ".")
                + "</p>",
        };

        return EmailLayout.Wrap($"""
            <p>{greeting}</p>
            {body}
            """);
    }
}
