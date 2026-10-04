using System.Globalization;
using System.Net;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.SharedKernel.Notifications.Templates;

namespace OnlineConsulting.Modules.Commerce.Application.Common.Templates;

public enum InvoiceEmailKind
{
    Issued,
    Receipt,
    Voided,
}

public record InvoiceEmailModel(InvoiceEmailKind Kind, InvoiceResponse Invoice, string BusinessName, string? ViewUrl);

/// <summary>An open invoice asks the customer to pay with a "View and pay" link; a receipt confirms the payment; a voided invoice tells them nothing is owed. All list every line and the totals.</summary>
public class InvoiceEmailTemplate : IEmailTemplate<InvoiceEmailModel>
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public string Subject(InvoiceEmailModel model) => model.Kind switch
    {
        InvoiceEmailKind.Receipt => $"Receipt for {model.Invoice.InvoiceNumber} ({Money(model.Invoice.Total)} paid)",
        InvoiceEmailKind.Voided => $"Invoice {model.Invoice.InvoiceNumber} from {model.BusinessName} was cancelled",
        _ => $"Invoice {model.Invoice.InvoiceNumber} from {model.BusinessName}: {Money(model.Invoice.Total)} due",
    };

    public string Build(InvoiceEmailModel model)
    {
        var invoice = model.Invoice;
        var firstName = invoice.BillToName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";

        var intro = model.Kind switch
        {
            InvoiceEmailKind.Receipt => $"Thanks for your payment! Here's your receipt for {invoice.Title}.",
            InvoiceEmailKind.Voided => $"Your invoice for {invoice.Title} was cancelled, so there's nothing to pay for it."
                + (string.IsNullOrWhiteSpace(invoice.VoidReason) ? "" : $" Reason: {invoice.VoidReason}"),
            _ => $"Here's your invoice for {invoice.Title}.",
        };

        var lines = string.Concat(invoice.Lines.Select(line => $"""
            <tr>
                <td style="padding: 8px 0; border-bottom: 1px solid #eeeeee;">{Encode(line.Description)}{(line.Quantity != 1 ? $" &times; {line.Quantity.ToString("0.##", Usd)}" : "")}</td>
                <td style="padding: 8px 0; border-bottom: 1px solid #eeeeee; text-align: right;">{Money(line.Subtotal)}</td>
            </tr>
            """));

        var discount = invoice.DiscountAmount > 0
            ? TotalRow(invoice.DiscountLabel ?? "Discount", $"-{Money(invoice.DiscountAmount)}", "#107C10")
            : "";

        var tax = invoice.TaxAmount > 0 ? TotalRow("Tax", Money(invoice.TaxAmount)) : "";

        var totalLabel = model.Kind switch
        {
            InvoiceEmailKind.Receipt => "Paid",
            InvoiceEmailKind.Voided => "Cancelled",
            _ => "Amount due",
        };

        var due = model.Kind == InvoiceEmailKind.Issued && invoice.DueAt is { } dueAt
            ? $"<p style=\"color: #777777;\">Due by {dueAt.ToString("MMMM d, yyyy", Usd)}.</p>"
            : "";

        var button = string.IsNullOrWhiteSpace(model.ViewUrl)
            ? ""
            : $"""
              <p style="margin: 24px 0;">
                  <a href="{Encode(model.ViewUrl)}" style="display: inline-block; padding: 12px 22px; border-radius: 999px; background: #FF9F1C; color: #2B1A00; font-weight: bold; text-decoration: none;">
                      {model.Kind switch { InvoiceEmailKind.Receipt => "View receipt", InvoiceEmailKind.Voided => "View invoice", _ => "View and pay" }}
                  </a>
              </p>
              """;

        return EmailLayout.Wrap($"""
            <p>{(string.IsNullOrWhiteSpace(firstName) ? "Hi," : $"Hi {Encode(firstName)},")}</p>
            <p>{Encode(intro)}</p>
            <p style="color: #777777; margin-bottom: 4px;">{Encode(invoice.InvoiceNumber)} &bull; issued {invoice.IssuedAt.ToString("MMMM d, yyyy", Usd)}</p>
            <table style="width: 100%; max-width: 520px; border-collapse: collapse; margin: 8px 0 16px;">
                {lines}
                {TotalRow("Subtotal", Money(invoice.Subtotal))}
                {discount}
                {tax}
                {TotalRow(totalLabel, Money(invoice.Total), bold: true)}
            </table>
            {due}
            {button}
            """);
    }

    private static string TotalRow(string label, string value, string? color = null, bool bold = false) => $"""
        <tr>
            <td style="padding: 6px 0; {(bold ? "font-weight: bold; font-size: 16px;" : "color: #555555;")}{(color is null ? "" : $" color: {color};")}">{Encode(label)}</td>
            <td style="padding: 6px 0; text-align: right; {(bold ? "font-weight: bold; font-size: 16px;" : "")}{(color is null ? "" : $" color: {color};")}">{value}</td>
        </tr>
        """;

    private static string Money(decimal amount) => amount.ToString("C", Usd);

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
