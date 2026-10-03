using System.Globalization;
using System.Net;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships;

/// <summary>Emails the member a receipt for a paid membership invoice - the provider's invoice stays the official document (number, PDF,
/// hosted page); this relays it with the plan's perks. A $0 first invoice is a trial start. Best effort: failures are logged.</summary>
public class MembershipReceiptSender(IEmailOutboxWriter<IMembershipsOutboxModule> outboxWriter, ISubscriptionGateway subscriptionGateway,
    IUserContactReader contactReader, IMembershipPlanRepository planRepository, ILogger<MembershipReceiptSender> logger)
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public async Task SendLatestAsync(CustomerMembership membership, CancellationToken cancellationToken = default)
    {
        if (membership.ProviderSubscriptionId is not { } providerSubscriptionId)
        {
            return;
        }

        try
        {
            if (await subscriptionGateway.GetLatestInvoiceAsync(providerSubscriptionId, cancellationToken) is { IsPaid: true } invoice)
            {
                await SendAsync(membership, invoice, cancellationToken);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Membership {MembershipId}: reading the latest invoice for the receipt failed.", membership.Id);
        }
    }

    public async Task SendAsync(CustomerMembership membership, SubscriptionInvoice invoice, CancellationToken cancellationToken = default)
    {
        try
        {
            var contact = await contactReader.GetContactAsync(membership.UserId, cancellationToken);
            if (contact?.Email is not { Length: > 0 } email)
            {
                return;
            }

            var plan = await planRepository.GetAsync(p => p.Id == membership.MembershipPlanId, cancellationToken: cancellationToken);
            var trialStart = invoice.AmountPaid == 0 && invoice.BillingReason == SubscriptionInvoice.FirstInvoiceReason;
            var subject = trialStart
                ? $"Your {plan?.Name ?? "membership"} free trial has started"
                : $"Your membership receipt{(invoice.Number is null ? "" : $" ({invoice.Number})")}: {Money(invoice.AmountPaid)} paid";

            await outboxWriter.EnqueueAsync(email, subject, Build(contact.FirstName, plan, invoice, trialStart),
                sourceReference: $"MembershipInvoice:{invoice.ProviderInvoiceId}", cancellationToken: cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Membership {MembershipId}: queuing the receipt for invoice {InvoiceId} failed.", membership.Id, invoice.ProviderInvoiceId);
        }
    }

    private static string Build(string firstName, MembershipPlan? plan, SubscriptionInvoice invoice, bool trialStart)
    {
        var planName = plan?.Name ?? "membership";
        var first = invoice.BillingReason == SubscriptionInvoice.FirstInvoiceReason;
        var intro = trialStart
            ? $"Welcome to {planName}! Your free trial has started and nothing was charged today."
                + (invoice.PeriodEnd is { } trialEnd ? $" Your first payment will be on {trialEnd.ToString("MMMM d, yyyy", Usd)} unless you cancel before then." : "")
            : first
                ? $"Welcome to {planName}! Thanks for joining. Here's the receipt for your first payment."
                : $"Your {planName} membership has renewed. Thanks for staying with us! Here's your receipt.";
        var period = !trialStart && invoice.PeriodStart is { } start && invoice.PeriodEnd is { } end
            ? $"<p style=\"color: #777777;\">Membership period: {start.ToString("MMM d, yyyy", Usd)} to {end.ToString("MMM d, yyyy", Usd)}</p>"
            : "";
        var perks = plan is null
            ? ""
            : $"""
              <p style="margin-top: 20px; font-weight: bold;">Your member perks</p>
              <ul style="margin-top: 4px; padding-left: 20px;">
                  <li>{plan.IncludedVisitsPerYear} included {(plan.IncludedVisitsPerYear == 1 ? "visit" : "visits")} per year</li>
                  {(plan.DiscountPercent > 0 ? $"<li>{plan.DiscountPercent:0.##}% off every additional service</li>" : "")}
                  <li>Priority booking</li>
              </ul>
              """;
        var lines = string.Concat(invoice.Lines.Select(line => $"""
            <tr>
                <td style="padding: 8px 0; border-bottom: 1px solid #eeeeee;">{Encode(line.Description)}</td>
                <td style="padding: 8px 0; border-bottom: 1px solid #eeeeee; text-align: right;">{Money(line.Amount)}</td>
            </tr>
            """));
        var links = string.Concat(
            invoice.HostedUrl is null ? "" : Button(invoice.HostedUrl, "View invoice", primary: true),
            invoice.PdfUrl is null ? "" : Button(invoice.PdfUrl, "Download PDF", primary: false));

        return EmailLayout.Wrap($"""
            <p>{(string.IsNullOrWhiteSpace(firstName) ? "Hi," : $"Hi {Encode(firstName)},")}</p>
            <p>{Encode(intro)}</p>
            <p style="color: #777777; margin-bottom: 4px;">{Encode(invoice.Number ?? "Invoice")} &bull; {DateTimeOffset.UtcNow.ToString("MMMM d, yyyy", Usd)}</p>
            {period}
            <table style="width: 100%; max-width: 520px; border-collapse: collapse; margin: 8px 0 16px;">
                {lines}
                <tr>
                    <td style="padding: 8px 0; font-weight: bold; font-size: 16px;">{(trialStart ? "Charged today" : "Paid")}</td>
                    <td style="padding: 8px 0; font-weight: bold; font-size: 16px; text-align: right;">{Money(invoice.AmountPaid)}</td>
                </tr>
            </table>
            {perks}
            <p style="margin: 24px 0;">{links}</p>
            <p style="color: #777777;">You can pause, switch or cancel your membership anytime from your account.</p>
            """);
    }

    private static string Button(string url, string label, bool primary) =>
        $"<a href=\"{Encode(url)}\" style=\"display: inline-block; margin: 0 8px 8px 0; padding: 12px 22px; border-radius: 999px; font-weight: bold; text-decoration: none; " +
        (primary ? "background: #FF9F1C; color: #2B1A00;" : "background: #EEF4FB; color: #0F6CBD;") + $"\">{Encode(label)}</a>";

    private static string Money(decimal amount) => amount.ToString("C", Usd);

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
