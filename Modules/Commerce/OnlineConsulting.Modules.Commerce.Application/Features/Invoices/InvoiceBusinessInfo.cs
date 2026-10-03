namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

/// <summary>Who the invoice is from - read from the "Billing" config section.</summary>
public sealed class InvoiceBusinessInfo
{
    public const string SectionName = "Billing";

    public string BusinessName { get; set; } = "ComfortPro";
    public string? BusinessAddress { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessPhone { get; set; }

    /// <summary>Web app origin used for "View and pay" links in invoice emails; falls back to Auth:ClientOrigin.</summary>
    public string? ClientOrigin { get; set; }

    public int PaymentTermsDays { get; set; } = 14;
}
