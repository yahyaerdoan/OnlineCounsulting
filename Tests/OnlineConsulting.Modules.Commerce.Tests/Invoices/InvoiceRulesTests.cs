using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Tests.Invoices;

public class InvoiceRulesTests
{
    [Theory]
    [InlineData(InvoiceStatuses.Open, true)]
    [InlineData(InvoiceStatuses.Paid, false)]
    [InlineData(InvoiceStatuses.Void, false)]
    public void IsOpen(string status, bool expected) =>
        Assert.Equal(expected, InvoiceRules.IsOpen(status));

    [Theory]
    [InlineData(InvoiceStatuses.Open, 10, true)]
    [InlineData(InvoiceStatuses.Open, 0, false)]
    [InlineData(InvoiceStatuses.Paid, 10, false)]
    [InlineData(InvoiceStatuses.Void, 10, false)]
    public void CanBePaidByCustomer(string status, decimal total, bool expected) =>
        Assert.Equal(expected, InvoiceRules.CanBePaidByCustomer(status, total));
}
