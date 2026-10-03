using System.Globalization;
using System.Net;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions;

/// <summary>Emails the tenant's primary contact a receipt for a paid subscription invoice. The provider's invoice stays the official document;
/// the email relays its number, amount and period, and links to the provider's hosted invoice page and PDF. Best effort: failures are logged.</summary>
public class TenantReceiptSender(IEmailOutboxWriter<ITenancyOutboxModule> outboxWriter, ISubscriptionGateway subscriptionGateway, ILogger<TenantReceiptSender> logger)
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public async Task SendLatestAsync(Tenant tenant, string providerSubscriptionId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await subscriptionGateway.GetLatestInvoiceAsync(providerSubscriptionId, cancellationToken) is { IsPaid: true } invoice)
            {
                await SendAsync(tenant, invoice, cancellationToken);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Tenant {TenantId}: reading the latest subscription invoice for the receipt failed.", tenant.Id);
        }
    }

    public async Task SendAsync(Tenant tenant, SubscriptionInvoice invoice, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = $"Your receipt from ComfortPro{(invoice.Number is null ? "" : $" ({invoice.Number})")}: {Money(invoice.AmountPaid)} paid";
            await outboxWriter.EnqueueAsync(tenant.PrimaryContactEmail, subject, Build(tenant, invoice),
                sourceReference: $"TenantInvoice:{invoice.ProviderInvoiceId}", cancellationToken: cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Tenant {TenantId}: queuing the receipt for invoice {InvoiceId} failed.", tenant.Id, invoice.ProviderInvoiceId);
        }
    }

    private static string Build(Tenant tenant, SubscriptionInvoice invoice)
    {
        var first = invoice.BillingReason == SubscriptionInvoice.FirstInvoiceReason;
        var intro = first
            ? $"Welcome aboard! Thanks for subscribing. Here's the receipt for {tenant.Name}'s first payment."
            : $"Thanks! Your subscription for {tenant.Name} has renewed. Here's your receipt.";
        var period = invoice.PeriodStart is { } start && invoice.PeriodEnd is { } end
            ? $"<p style=\"color: #777777;\">Service period: {start.ToString("MMM d, yyyy", Usd)} to {end.ToString("MMM d, yyyy", Usd)}</p>"
            : "";
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
            <p>Hi,</p>
            <p>{Encode(intro)}</p>
            <p style="color: #777777; margin-bottom: 4px;">{Encode(invoice.Number ?? "Invoice")} &bull; paid {DateTimeOffset.UtcNow.ToString("MMMM d, yyyy", Usd)}</p>
            {period}
            <table style="width: 100%; max-width: 520px; border-collapse: collapse; margin: 8px 0 16px;">
                {lines}
                <tr>
                    <td style="padding: 8px 0; font-weight: bold; font-size: 16px;">Paid</td>
                    <td style="padding: 8px 0; font-weight: bold; font-size: 16px; text-align: right;">{Money(invoice.AmountPaid)}</td>
                </tr>
            </table>
            <p style="margin: 24px 0;">{links}</p>
            <p style="color: #777777;">Keep this email for your records. You can manage your modules and billing from your admin dashboard.</p>
            """);
    }

    private static string Button(string url, string label, bool primary) =>
        $"<a href=\"{Encode(url)}\" style=\"display: inline-block; margin: 0 8px 8px 0; padding: 12px 22px; border-radius: 999px; font-weight: bold; text-decoration: none; " +
        (primary ? "background: #FF9F1C; color: #2B1A00;" : "background: #EEF4FB; color: #0F6CBD;") + $"\">{Encode(label)}</a>";

    private static string Money(decimal amount) => amount.ToString("C", Usd);

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
