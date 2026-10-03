namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;

/// <summary>Card is an online payment; Cash and Check are recorded by staff; Covered is a zero-total invoice (e.g. fully discounted).</summary>
public static class InvoicePaymentMethods
{
    public const string Card = "Card";
    public const string Cash = "Cash";
    public const string Check = "Check";
    public const string Covered = "Covered";

    public static readonly string[] Offline = [Cash, Check, Card];
}
