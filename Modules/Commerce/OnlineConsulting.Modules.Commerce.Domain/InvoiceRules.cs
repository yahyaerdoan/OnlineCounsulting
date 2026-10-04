namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>Invoice lifecycle rules shared by <see cref="Invoice"/> and code that only has the status (e.g. HATEOAS links).</summary>
public static class InvoiceRules
{
    /// <summary>Open invoices can be paid or voided.</summary>
    public static bool IsOpen(string status) => status == InvoiceStatuses.Open;

    /// <summary>Open with something left to pay, so the customer can pay it by card.</summary>
    public static bool CanBePaidByCustomer(string status, decimal total) => IsOpen(status) && total > 0;
}
