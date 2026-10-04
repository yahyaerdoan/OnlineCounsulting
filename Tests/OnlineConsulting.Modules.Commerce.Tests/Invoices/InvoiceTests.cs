using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Tests.Invoices;

public class InvoiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);
    private static readonly InvoiceBillTo BillTo = new("Jane Doe", "jane@example.test", "1 Main St");

    private static Invoice Issue(params InvoiceCharge[] charges) => IssueWith(null, charges).Invoice;

    private static (Invoice Invoice, IReadOnlyList<InvoiceLine> Lines) IssueWith(InvoiceDiscount? discount, params InvoiceCharge[] charges) =>
        Invoice.Issue("INV-2610-ABC123", Guid.NewGuid(), InvoiceSources.Appointment, Guid.NewGuid(), "Service visit: Duct cleaning", "USD", BillTo,
            charges, discount, Now, Now.AddDays(14));

    private static Invoice IssueOpen() => Issue(new InvoiceCharge("Duct cleaning", 1, 100m, 0));

    [Fact]
    public void Issue_CreatesOpenInvoiceWithCopiedDetails()
    {
        var invoice = IssueOpen();

        Assert.Equal(InvoiceStatuses.Open, invoice.Status);
        Assert.True(invoice.IsOpen);
        Assert.True(invoice.CanBePaidByCustomer);
        Assert.Equal("Jane Doe", invoice.BillToName);
        Assert.Equal("jane@example.test", invoice.BillToEmail);
        Assert.Equal("1 Main St", invoice.BillToAddress);
        Assert.Equal(Now, invoice.IssuedAt);
        Assert.Equal(Now.AddDays(14), invoice.DueAt);
        Assert.Null(invoice.PaidAt);
    }

    [Fact]
    public void Issue_BuildsOrderedLinesOwnedByTheInvoice()
    {
        var (invoice, lines) = IssueWith(null, new InvoiceCharge("  Filter  ", 2, 10m, 0), new InvoiceCharge("Labor", 1, 50m, 0));

        Assert.Equal(2, lines.Count);
        Assert.All(lines, l => Assert.Equal(invoice.Id, l.InvoiceId));
        Assert.Equal([0, 1], lines.Select(l => l.SortOrder));
        Assert.Equal("Filter", lines[0].Description);
    }

    [Fact]
    public void Issue_AppliesDiscountBeforeTaxAndSumsRoundedLines()
    {
        var (invoice, lines) = IssueWith(new InvoiceDiscount(10, "Member discount"), new InvoiceCharge("Filter", 3, 3.335m, 8), new InvoiceCharge("Labor", 1, 100m, 0));

        Assert.Equal(10.01m, lines[0].Subtotal);
        Assert.Equal(1.00m, lines[0].DiscountAmount);
        Assert.Equal(0.72m, lines[0].TaxAmount);
        Assert.Equal(9.73m, lines[0].Total);

        Assert.Equal(110.01m, invoice.Subtotal);
        Assert.Equal(11.00m, invoice.DiscountAmount);
        Assert.Equal(0.72m, invoice.TaxAmount);
        Assert.Equal(99.73m, invoice.Total);
        Assert.Equal("Member discount", invoice.DiscountLabel);
    }

    [Fact]
    public void Issue_WithoutCharges_Throws() =>
        Assert.Throws<ArgumentException>(() => IssueWith(null));

    [Fact]
    public void Issue_WithUnknownSource_Throws() =>
        Assert.Throws<ArgumentException>(() => Invoice.Issue("INV-1", Guid.NewGuid(), "Membership", Guid.NewGuid(), "Title", "USD", BillTo,
            [new InvoiceCharge("Item", 1, 1m, 0)], null, Now, null));

    public static TheoryData<InvoiceCharge> InvalidCharges => new()
    {
        new InvoiceCharge(" ", 1, 1m, 0),
        new InvoiceCharge("Item", 0, 1m, 0),
        new InvoiceCharge("Item", 1, -1m, 0),
        new InvoiceCharge("Item", 1, 1m, -1),
        new InvoiceCharge("Item", 1, 1m, 101),
    };

    [Theory]
    [MemberData(nameof(InvalidCharges))]
    public void Issue_WithInvalidCharge_Throws(InvoiceCharge charge) =>
        Assert.ThrowsAny<ArgumentException>(() => Issue(charge));

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Issue_WithInvalidDiscount_Throws(decimal percent) =>
        Assert.ThrowsAny<ArgumentException>(() => IssueWith(new InvoiceDiscount(percent, "Discount"), new InvoiceCharge("Item", 1, 1m, 0)));

    [Fact]
    public void Issue_WithZeroTotal_CannotBePaidByCustomer()
    {
        var invoice = Issue(new InvoiceCharge("Free check", 1, 0m, 0));

        Assert.True(invoice.IsOpen);
        Assert.False(invoice.CanBePaidByCustomer);
    }

    [Fact]
    public void MarkPaid_WhenOpen_SettlesWithMethodAndTime()
    {
        var invoice = IssueOpen();

        invoice.MarkPaid(InvoicePaymentMethods.Cash, null, null, Now);

        Assert.Equal(InvoiceStatuses.Paid, invoice.Status);
        Assert.Equal(InvoicePaymentMethods.Cash, invoice.PaymentMethod);
        Assert.Equal(Now, invoice.PaidAt);
        Assert.False(invoice.IsOpen);
        Assert.False(invoice.CanBePaidByCustomer);
    }

    [Fact]
    public void MarkPaid_KeepsStartedPaymentWhenNoneGiven()
    {
        var invoice = IssueOpen();
        invoice.StartCardPayment("Stripe", "pi_1");

        invoice.MarkPaid(InvoicePaymentMethods.Card, null, null, Now);

        Assert.Equal("Stripe", invoice.PaymentProvider);
        Assert.Equal("pi_1", invoice.ProviderPaymentId);
    }

    [Fact]
    public void MarkPaid_WithUnknownMethod_Throws() =>
        Assert.Throws<ArgumentException>(() => IssueOpen().MarkPaid("Bitcoin", null, null, Now));

    [Fact]
    public void MarkPaid_CoveredWithAmountDue_Throws() =>
        Assert.Throws<InvalidOperationException>(() => IssueOpen().MarkPaid(InvoicePaymentMethods.Covered, null, null, Now));

    [Fact]
    public void MarkPaid_CoveredWithZeroTotal_Settles()
    {
        var invoice = Issue(new InvoiceCharge("Free check", 1, 0m, 0));

        invoice.MarkPaid(InvoicePaymentMethods.Covered, null, null, Now);

        Assert.Equal(InvoiceStatuses.Paid, invoice.Status);
    }

    [Fact]
    public void MarkPaid_WhenAlreadyPaid_Throws()
    {
        var invoice = IssueOpen();
        invoice.MarkPaid(InvoicePaymentMethods.Cash, null, null, Now);

        _ = Assert.Throws<InvalidOperationException>(() => invoice.MarkPaid(InvoicePaymentMethods.Card, "Stripe", "pi_1", Now));
    }

    [Fact]
    public void MarkPaid_WhenVoid_Throws()
    {
        var invoice = IssueOpen();
        invoice.Void(null);

        _ = Assert.Throws<InvalidOperationException>(() => invoice.MarkPaid(InvoicePaymentMethods.Cash, null, null, Now));
    }

    [Fact]
    public void StartCardPayment_WhenPayable_RecordsPaymentAndStaysOpen()
    {
        var invoice = IssueOpen();

        invoice.StartCardPayment("Stripe", "pi_1");

        Assert.Equal("Stripe", invoice.PaymentProvider);
        Assert.Equal("pi_1", invoice.ProviderPaymentId);
        Assert.True(invoice.IsOpen);
    }

    [Fact]
    public void StartCardPayment_WithZeroTotal_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Issue(new InvoiceCharge("Free check", 1, 0m, 0)).StartCardPayment("Stripe", "pi_1"));

    [Fact]
    public void StartCardPayment_WhenPaid_Throws()
    {
        var invoice = IssueOpen();
        invoice.MarkPaid(InvoicePaymentMethods.Cash, null, null, Now);

        _ = Assert.Throws<InvalidOperationException>(() => invoice.StartCardPayment("Stripe", "pi_1"));
    }

    [Fact]
    public void Void_WhenOpen_SetsTrimmedReason()
    {
        var invoice = IssueOpen();

        invoice.Void("  Duplicate  ");

        Assert.Equal(InvoiceStatuses.Void, invoice.Status);
        Assert.Equal("Duplicate", invoice.VoidReason);
        Assert.False(invoice.IsOpen);
    }

    [Fact]
    public void Void_WithBlankReason_StoresNull()
    {
        var invoice = IssueOpen();

        invoice.Void(" ");

        Assert.Null(invoice.VoidReason);
    }

    [Fact]
    public void Void_WhenPaid_Throws()
    {
        var invoice = IssueOpen();
        invoice.MarkPaid(InvoicePaymentMethods.Cash, null, null, Now);

        _ = Assert.Throws<InvalidOperationException>(() => invoice.Void(null));
    }
}
