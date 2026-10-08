using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>
/// The bill for one paid order or one completed visit, the aggregate root of its <see cref="Lines"/>. State changes only through its methods, which throw
/// when called in the wrong state; load it with its lines when they're needed and save them together.
/// </summary>
public class Invoice : SequentialGuidTenantEntity
{
    private readonly List<InvoiceLine> _lines = [];

    private Invoice()
    {
    }

    public string InvoiceNumber { get; private set; } = string.Empty;

    /// <summary>Identity module user id, no navigation.</summary>
    public Guid UserId { get; private set; }

    /// <summary>One of <see cref="InvoiceSources"/>.</summary>
    public string SourceType { get; private set; } = string.Empty;
    public Guid SourceId { get; private set; }

    /// <summary>One of <see cref="InvoiceStatuses"/>.</summary>
    public string Status { get; private set; } = InvoiceStatuses.Open;

    public string Currency { get; private set; } = string.Empty;
    public string BillToName { get; private set; } = string.Empty;
    public string? BillToEmail { get; private set; }
    public string? BillToAddress { get; private set; }

    /// <summary>One-line reason shown in lists and emails, the order number or "Service visit: Duct cleaning".</summary>
    public string Title { get; private set; } = string.Empty;

    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public string? DiscountLabel { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal Total { get; private set; }

    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset? DueAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    /// <summary>One of <see cref="InvoicePaymentMethods"/> once paid.</summary>
    public string? PaymentMethod { get; private set; }
    public string? PaymentProvider { get; private set; }
    public string? ProviderPaymentId { get; private set; }
    public string? VoidReason { get; private set; }

    /// <summary>The charges in billing order; fixed once issued.</summary>
    public IReadOnlyList<InvoiceLine> Lines => _lines;

    /// <summary>Open, so it can be paid or voided.</summary>
    public bool IsOpen => InvoiceRules.IsOpen(Status);

    /// <summary>Open with something left to pay.</summary>
    public bool CanBePaidByCustomer => InvoiceRules.CanBePaidByCustomer(Status, Total);

    /// <summary>Creates an open invoice with its lines; totals are the sums of the rounded line amounts.</summary>
    public static Invoice Issue(string invoiceNumber, Guid userId, string sourceType, Guid sourceId, string title,
        string currency, InvoiceBillTo billTo, IReadOnlyList<InvoiceCharge> charges, InvoiceDiscount? discount, DateTimeOffset issuedAt, DateTimeOffset? dueAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(invoiceNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        ArgumentException.ThrowIfNullOrWhiteSpace(billTo.Name);

        if (sourceType is not (InvoiceSources.Order or InvoiceSources.Appointment))
        {
            throw new ArgumentException($"Unknown invoice source '{sourceType}'.", nameof(sourceType));
        }

        if (charges.Count == 0)
        {
            throw new ArgumentException("An invoice needs at least one charge.", nameof(charges));
        }

        var discountPercent = discount?.Percent ?? 0;

        ArgumentOutOfRangeException.ThrowIfNegative(discountPercent);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(discountPercent, 100);

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            UserId = userId,
            SourceType = sourceType,
            SourceId = sourceId,
            Title = title,
            Currency = currency,
            BillToName = billTo.Name,
            BillToEmail = billTo.Email,
            BillToAddress = billTo.Address,
            DiscountLabel = discount?.Label,
            IssuedAt = issuedAt,
            DueAt = dueAt,
        };

        invoice._lines.AddRange(charges.Select((charge, index) => InvoiceLine.Create(invoice.Id, index, charge, discountPercent)));
        invoice.Subtotal = invoice._lines.Sum(l => l.Subtotal);
        invoice.DiscountAmount = invoice._lines.Sum(l => l.DiscountAmount);
        invoice.TaxAmount = invoice._lines.Sum(l => l.TaxAmount);
        invoice.Total = invoice._lines.Sum(l => l.Total);

        return invoice;
    }

    /// <summary>Records the card payment the customer started; it stays open until the payment succeeds. Requires <see cref="CanBePaidByCustomer"/>.</summary>
    public void StartCardPayment(string paymentProvider, string providerPaymentId)
    {
        if (!CanBePaidByCustomer)
        {
            throw new InvalidOperationException($"Invoice {InvoiceNumber} cannot be paid: it is {Status} with a total of {Total}.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(paymentProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerPaymentId);

        PaymentProvider = paymentProvider;
        ProviderPaymentId = providerPaymentId;
    }

    /// <summary>Settles the invoice; <see cref="InvoicePaymentMethods.Covered"/> only for a zero total. Requires <see cref="IsOpen"/>.</summary>
    public void MarkPaid(string paymentMethod, string? paymentProvider, string? providerPaymentId, DateTimeOffset paidAt)
    {
        EnsureOpen(nameof(MarkPaid));

        if (!InvoicePaymentMethods.All.Contains(paymentMethod))
        {
            throw new ArgumentException($"Unknown payment method '{paymentMethod}'.", nameof(paymentMethod));
        }

        if (paymentMethod == InvoicePaymentMethods.Covered && Total > 0)
        {
            throw new InvalidOperationException($"Invoice {InvoiceNumber} has {Total} to pay, so it cannot be marked as covered.");
        }

        Status = InvoiceStatuses.Paid;
        PaidAt = paidAt;
        PaymentMethod = paymentMethod;
        PaymentProvider = paymentProvider ?? PaymentProvider;
        ProviderPaymentId = providerPaymentId ?? ProviderPaymentId;
    }

    /// <summary>Cancels the invoice so nothing is owed. Requires <see cref="IsOpen"/>.</summary>
    public void Void(string? reason)
    {
        EnsureOpen(nameof(Void));
        Status = InvoiceStatuses.Void;
        VoidReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }

    private void EnsureOpen(string action)
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException($"{action} needs an open invoice, but invoice {InvoiceNumber} is {Status}.");
        }
    }
}
