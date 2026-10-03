namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Constants;

public static class InvoiceMessages
{
    public const string NotFound = "Invoice not found.";
    public const string OnlyOpenCanBePaid = "Only an open invoice can be paid.";
    public const string OnlyOpenCanBeVoided = "Only an open invoice can be voided.";
    public const string NothingToPay = "This invoice has nothing left to pay.";
    public const string PaymentSetupFailed = "We couldn't start the payment. Please try again in a moment.";
}
