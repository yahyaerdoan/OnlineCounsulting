namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>Values of <see cref="Invoice.PaymentMethod"/>: Card online or on site, Cash and Check recorded by staff, Covered for a zero total.</summary>
public static class InvoicePaymentMethods
{
    public const string Card = "Card";
    public const string Cash = "Cash";
    public const string Check = "Check";
    public const string Covered = "Covered";

    public static readonly string[] Offline = [Cash, Check, Card];

    public static readonly string[] All = [Card, Cash, Check, Covered];
}
