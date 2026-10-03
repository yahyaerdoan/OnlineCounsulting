using System.Globalization;
using System.Net;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Referrals.Application.Common.Templates;

public record ReferralRewardEmailModel(string? FirstName, decimal Amount);

/// <summary>Sent to the referrer when a referral is completed and its reward is added as account credit.</summary>
public class ReferralRewardTemplate : IEmailTemplate<ReferralRewardEmailModel>
{
    public string Subject(ReferralRewardEmailModel model) => $"You earned {Money(model.Amount)} in account credit";

    public string Build(ReferralRewardEmailModel model) => EmailLayout.Wrap($"""
        <p>{(string.IsNullOrWhiteSpace(model.FirstName) ? "Hi," : $"Hi {WebUtility.HtmlEncode(model.FirstName)},")}</p>
        <p>Thanks for spreading the word! Someone you referred booked their first service, so we added <strong>{Money(model.Amount)}</strong> to your account credit.</p>
        <p>It's applied automatically at your next checkout or membership payment.</p>
        """);

    private static string Money(decimal amount) => amount.ToString("C", CultureInfo.GetCultureInfo("en-US"));
}
